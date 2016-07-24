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

//DocumentsCoded.Where(d => d.DateCreated >= DateTime.Parse("1/1/2016")  && d.AccountId == "020d0cf0-29a2-4d45-9f12-c9a2c4318ca8").Dump();
var all = MonthlyDocuments.ToList();
all.Where (a => a.PartitionKey.Contains("2016") && a.CompanyName == "EFile").OrderBy (a => a.Timestamp).Dump();
var totalDocs = all.Select(a => a.RowKey).ToArray();
totalDocs.Distinct().Count().Dump("Total Koded");

var allIsolated = BatchesIsolated.ToList();
var allUnique = allIsolated.Where (i => i.AccountId == "020d0cf0-29a2-4d45-9f12-c9a2c4318ca8" && i.OldBatchValues == null);
allUnique.OrderBy (u => u.Timestamp).Dump();
allUnique.Sum(a => a.DocumentsCreated).Dump("Total Isolated");