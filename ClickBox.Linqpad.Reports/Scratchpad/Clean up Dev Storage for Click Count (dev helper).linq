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


//clean up batches isolated
var BatchesIsolatedTableClient = BatchesIsolated;

Console.WriteLine(BatchesIsolatedTableClient.CloudTable.Name);

foreach(var b in BatchesIsolated){
	Console.WriteLine(b);
	TableOperation deleteOperation = TableOperation.Delete(b);
   	BatchesIsolatedTableClient.CloudTable.Execute(deleteOperation);
}

//clean up documents coded

var DocumentsCodedTableClient = DocumentsCoded;

Console.WriteLine(DocumentsCodedTableClient.CloudTable.Name);

foreach(var d in DocumentsCoded){
	Console.WriteLine(d);
	TableOperation deleteOperation = TableOperation.Delete(d);
   	DocumentsCodedTableClient.CloudTable.Execute(deleteOperation);
}

//clean up Monthy batches

var MonthlyBatchesTableClient = MonthlyBatches;

Console.WriteLine(MonthlyBatchesTableClient.CloudTable.Name);

foreach(var mb in MonthlyBatches){
	Console.WriteLine(mb);
	TableOperation deleteOperation = TableOperation.Delete(mb);
   	MonthlyBatchesTableClient.CloudTable.Execute(deleteOperation);
}

//clean up Monthly documents coded

var MonthlyDocumentsTableClient = MonthlyDocuments;

Console.WriteLine(MonthlyDocumentsTableClient.CloudTable.Name);

foreach(var md in MonthlyDocuments){
	Console.WriteLine(md);
	TableOperation deleteOperation = TableOperation.Delete(md);
   	MonthlyDocumentsTableClient.CloudTable.Execute(deleteOperation);
}



