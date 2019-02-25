<Query Kind="Statements">
  <Connection>
    <ID>898042e4-f676-4c29-8008-4d76cbad8f62</ID>
    <Persist>true</Persist>
    <Driver Assembly="Madd0.AzureStorageDriver" PublicKeyToken="47842961fb3025d7">Madd0.AzureStorageDriver.AzureDriver</Driver>
    <DriverData>
      <UseHttps>true</UseHttps>
      <NumberOfRows>100</NumberOfRows>
      <AccountName>clickbox</AccountName>
      <AccountKey>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAfrBKKxNOUkujb+4N87yM5wAAAAACAAAAAAAQZgAAAAEAACAAAABhXo/MUxfdEvRJT0peWDHa7w/V+XuMFvSIpoccAxH+1gAAAAAOgAAAAAIAACAAAADmy3WFCUIo6Txs2BwmI37Ll5R6TIC186V/RE1QA91A6WAAAABwveedq2mLuxxH05Y/1AkDvOHT8elInkNJOEYHP3tUPXJA0VosgWQr5RZLWYTiXYZIgKhv1P0IEMJj0IPQV2EICQ3ZUt/egFXG7zc7AfmHWB37rY5E7WtIyiGKUOjTcU5AAAAAAX+OOny6MmpYG2Y4jQeA6sWiBEm2NFOmZ0+A5z5ARYLHrrw1y9rQTXhM+kugLQ7AeqFQA4MkmZBoMawdxFgofg==</AccountKey>
    </DriverData>
  </Connection>
</Query>

//from m in MonthlyBatches
//where m.CompanyName == "QCAT"
//select new {a = m.Timestamp, b = m.Timestamp.ToLocalTime()} 

//BATCHES ISOLATED THIS MONTH

var thisMonth = DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
var companyName = "Law Image";

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

var allDocsKodedThisMonthBy = MonthlyDocuments.Where (mb => mb.PartitionKey == thisMonth && mb.CompanyName == companyName);
var countDocsKodedThisMonthBy = allDocsKodedThisMonthBy.ToList().Count();
var numberKodedByTitle = string.Format("ALL {0} DOCUMENTS KODED THIS MONTH BY {1}", countDocsKodedThisMonthBy, companyName);
allDocsKodedThisMonthBy.Dump(numberKodedByTitle);







