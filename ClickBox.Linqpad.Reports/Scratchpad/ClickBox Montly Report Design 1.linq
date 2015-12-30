<Query Kind="Statements">
  <Connection>
    <ID>c7cbf417-e23f-4c46-9318-4bcd0860c0a5</ID>
    <Persist>true</Persist>
    <Driver Assembly="Madd0.AzureStorageDriver" PublicKeyToken="47842961fb3025d7">Madd0.AzureStorageDriver.AzureDriver</Driver>
    <DriverData>
      <UseHttps>true</UseHttps>
      <NumberOfRows>100</NumberOfRows>
      <UseLocalStorage>false</UseLocalStorage>
      <AccountName>clickboxdev</AccountName>
      <AccountKey>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAASuj6IA5IykWEyFcFK9OzEQAAAAACAAAAAAAQZgAAAAEAACAAAADa/w+/R6QfA//votbv83jDGo/8SNAs4dSCn/wn0NZDEQAAAAAOgAAAAAIAACAAAADqMtrKSE2qbBj//yQCIzsS3z1VFk5apwBGap1fvqeSimAAAAAkgNik9jhOdSX2jKgxWiOQiTFDlGx5jyatdHtIAB/06F0mQpb/2l9vm8tyKZXN0xehv7BgCjaOjNhZYPkuLnRyn6XXKvPKWZsmuu0QjJQHZeHV6lIqHLFhmjh1+yyEnKJAAAAAlX6b7icQlmz0foTfbFy6+lZQ0gjvF1MlVCN87dWXtDE707kXMMeX9lz5lFI7HZFVX6BRh05iP+1yU5yC6aMP2w==</AccountKey>
    </DriverData>
  </Connection>
</Query>

//from m in MonthlyBatches
//where m.CompanyName == "QCAT"
//select new {a = m.Timestamp, b = m.Timestamp.ToLocalTime()} 

//BATCHES ISOLATED THIS MONTH

var thisMonth = DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
var companyName = "QCAT";

//ALL BATCHES ISOLATED THIS MONTH

var allIsolatedThisMonth = MonthlyBatches.Where (mb => mb.PartitionKey == thisMonth);
var numberOfAllIsolated = allIsolatedThisMonth.ToList().Count();
var allIsolatedThisMonthTitle = string.Format("ALL {0} BATCHE(S) ISOLATED THIS MONTH",numberOfAllIsolated);
allIsolatedThisMonth.Dump(allIsolatedThisMonthTitle);

//BATCHES ISOLATED THIS MONTH BY COMPANY

var isolatedByCompanyNameThisMonth = MonthlyBatches.Where (mb => mb.PartitionKey == thisMonth && mb.CompanyName == companyName);
var numberOfIsolatedByCompanyNameThisMonth = isolatedByCompanyNameThisMonth.ToList().Count();
var allIsolatedByCompanyThisMonthTitle = string.Format("ALL {0} BATCHE(S) ISOLATED THIS MONTH BY {1}",numberOfIsolatedByCompanyNameThisMonth, companyName);
isolatedByCompanyNameThisMonth.Dump(allIsolatedByCompanyThisMonthTitle);

//ALL DOCUMENTS KODED THIS MONTH

var allDocumentsKodedThisMonth = MonthlyDocuments.Where (mb => mb.PartitionKey == thisMonth);
var numberOfAllDocsKodedThisMonth = allDocumentsKodedThisMonth.ToList().Count();
var numberOfAllDocsThisMonthTitle = string.Format("ALL {0} DOCUMENTS KODED THIS MONTH", numberOfAllDocsKodedThisMonth);
allDocumentsKodedThisMonth.Dump(numberOfAllDocsThisMonthTitle);

//DOCUMENTS KODED THIS MONTH BY COMPANY

MonthlyDocuments.Where (mb => mb.PartitionKey == thisMonth && mb.CompanyName == companyName).Dump("ALL DOCUMENTS KODED BY " +companyName+ " MONTH");