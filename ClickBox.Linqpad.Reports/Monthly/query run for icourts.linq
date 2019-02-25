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
  <Output>DataGrids</Output>
</Query>

var all = MonthlyDocuments.ToList();
all.Where (a => a.PartitionKey.Contains("2016") && a.CompanyName == "iCourts" && a.Timestamp.Date >= DateTimeOffset.Parse("1/10/2016").Date && a.Timestamp.Date < DateTimeOffset.Parse("1/01/2017").Date).Dump("Monthly Documents");

var allIsolated = BatchesIsolated.ToList();
allIsolated.Where (i => i.AccountId == "e9f9d179-dd4a-4d18-a344-88983e9ed6cc" && i.Timestamp.Date >= DateTimeOffset.Parse("1/10/2016").Date && i.Timestamp.Date < DateTimeOffset.Parse("1/01/2017").Date).Dump("Batches Isolated");