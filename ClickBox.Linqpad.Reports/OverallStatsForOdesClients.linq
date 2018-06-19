<Query Kind="Program">
  <Connection>
    <ID>2a46f8db-c918-4444-b14b-662f37caab4d</ID>
    <Persist>true</Persist>
    <Driver Assembly="Madd0.AzureStorageDriver" PublicKeyToken="47842961fb3025d7">Madd0.AzureStorageDriver.AzureDriver</Driver>
    <DriverData>
      <UseHttps>true</UseHttps>
      <NumberOfRows>100</NumberOfRows>
      <AccountKey>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAASRAkRAJZa0+FJbz2IIO7AgAAAAACAAAAAAAQZgAAAAEAACAAAABIiO5kFqy67BeRph4wS4yW4am7GNZxY7ghIDAJQA+58AAAAAAOgAAAAAIAACAAAADRz7D7DnpOuz5DEfbxb9TBIQjyBk7+RxzCqJpOWuaiUWAAAAAQxpXXcHm6X+Nj3obi1sw4dzsDdCzm+IjSJXRkYPghAQKdKwlNo49OXAyn0JzSl+IHccFQiSLvln+rtBf5LiTvnjyF+CfSjrcsFbSycKWFxFmM+MCVJkSsP2Fu6PMadJFAAAAAvAWVE4TrvUb1+J/7tE1I3PULIGXtkhN+CD7jKy7fv9bYGent07qEdkzGq8rhSQ/N71+suBAtM44r0feGxZW+ww==</AccountKey>
      <AccountName>clickbox</AccountName>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	//from m in MonthlyBatches
	//where m.CompanyName == "QCAT"
	//select new {a = m.Timestamp, b = m.Timestamp.ToLocalTime()} 
	var companies = UserAccounts.Where(s => s.Product =="QCAT-Odes").Select(s => s.CompanyName).ToList();
	//BATCHES ISOLATED THIS MONTH
	var months = MonthsBetween(new DateTime(2015,7,1), new DateTime(2018,7,1));
	
	var thisMonth = DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
	//var companyName = "Law Image";
	
	var companyStats = new List<MonthStats>();
	foreach (var company in companies.Where(c => c == "Law Image").ToList())
	{
		foreach (var month in months)
		{
			var stat = sss(month, company);
			if(stat.Batches != 0 || stat.Documents != 0)
			{
				companyStats.Add(stat);
			}
		}	
	}
	companyStats.Dump();
}

public class MonthStats
{
	public string CompanyName { get; set; }
	
	public DateTime Month { get; set; }
	
	public long Documents { get; set; }
	
	public long Batches { get; set; }
	
	public long IsolatedDocuments {get; set;}

	public double KodedDocumentsCost
	{
		get { return this.Documents * 0.09; }
	}
	
	public double IsolatedDocumentsCost
	{
		get { return this.Documents * 0.04; }
	}

}

public MonthStats sss(Tuple<DateTime, string> month, string companyName)
{
	var ms = new MonthStats(){ Month = month.Item1, CompanyName = companyName};
	//	//ALL BATCHES ISOLATED THIS MONTH
	//	
	//	var allIsolatedThisMonth = MonthlyBatches.Where (mb => mb.PartitionKey == thisMonth);
	//	var numberOfAllIsolated = allIsolatedThisMonth.ToList().Count();
	//	var allIsolatedThisMonthTitle = string.Format("ALL {0} BATCHE(S) ISOLATED THIS MONTH",numberOfAllIsolated);
	//	allIsolatedThisMonth.Dump(allIsolatedThisMonthTitle);
	//	
	//BATCHES ISOLATED THIS MONTH BY COMPANY

	var isolatedByCompanyNameThisMonth = MonthlyBatches.Where(mb => mb.PartitionKey == month.Item2 && mb.CompanyName == companyName);
	var numberOfIsolatedByCompanyNameThisMonth = isolatedByCompanyNameThisMonth.Select(bcntm => bcntm.DocumentCount).ToList();
	//var allIsolatedByCompanyThisMonthTitle = string.Format("ALL {0} BATCHE(S) ISOLATED THIS MONTH BY {1}", numberOfIsolatedByCompanyNameThisMonth, companyName);
	// isolatedByCompanyNameThisMonth.Dump(allIsolatedByCompanyThisMonthTitle);
	// allIsolatedByCompanyThisMonthTitle.Dump();
	ms.Batches = numberOfIsolatedByCompanyNameThisMonth.Count();
	ms.IsolatedDocuments =numberOfIsolatedByCompanyNameThisMonth.Sum().Value;

	//	//ALL DOCUMENTS KODED THIS MONTH
	//	
	//	var allDocumentsKodedThisMonth = MonthlyDocuments.Where (mb => mb.PartitionKey == thisMonth);
	//	var numberOfAllDocsKodedThisMonth = allDocumentsKodedThisMonth.ToList().Count();
	//	var numberOfAllDocsThisMonthTitle = string.Format("ALL {0} DOCUMENTS KODED THIS MONTH", numberOfAllDocsKodedThisMonth);
	//	allDocumentsKodedThisMonth.Dump(numberOfAllDocsThisMonthTitle);
	//	
	//DOCUMENTS KODED THIS MONTH BY COMPANY

	var allDocsKodedThisMonthBy = MonthlyDocuments.Where(mb => mb.PartitionKey == month.Item2 && mb.CompanyName == companyName);
	var countDocsKodedThisMonthBy = allDocsKodedThisMonthBy.ToList();
	//var numberKodedByTitle = string.Format("ALL {0} DOCUMENTS KODED THIS MONTH BY {1}", countDocsKodedThisMonthBy, companyName);
	// allDocsKodedThisMonthBy.Dump(numberKodedByTitle);
	// numberKodedByTitle.Dump();
	ms.Documents = countDocsKodedThisMonthBy.Count();
	countDocsKodedThisMonthBy.GroupBy(dktmb => dktmb.ProjectId).Count().Dump();
	
	return ms;
}

public static IEnumerable< Tuple<DateTime, string>> MonthsBetween(DateTime startDate, DateTime endDate)
{
	DateTime iterator;
	DateTime limit;

	if (endDate > startDate)
	{
		iterator = new DateTime(startDate.Year, startDate.Month, 1);
		limit = endDate;
	}
	else
	{
		iterator = new DateTime(endDate.Year, endDate.Month, 1);
		limit = startDate;
	}

	//var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
	while (iterator <= limit)
	{
		yield return Tuple.Create(iterator, iterator.Month.ToString() + iterator.Year.ToString());
		iterator = iterator.AddMonths(1);
	}
}
// Define other methods and classes here
