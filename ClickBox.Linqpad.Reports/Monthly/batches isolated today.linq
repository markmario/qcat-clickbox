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

var i = BatchesIsolated.Where (bi => bi.DateCreated > DateTime.Now.AddDays(-1)).ToArray();
var k = i.OrderBy (x => x.DateCreated).ToArray();
k.ToList().ForEach(b => b.DateCreated = new Nullable<DateTime>(b.DateCreated.Value.ToLocalTime));
k.Dump();
//var j = k.
//j.DateCreated.Value.Dump();
//j.DateCreated.Value.ToLocalTime().Dump();