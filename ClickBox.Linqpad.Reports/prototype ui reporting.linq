<Query Kind="Statements">
  <Reference>&lt;MyDocuments&gt;\Visual Studio 2015\Projects\LinqPadUiPrototype\LinqPadUiPrototype\bin\Debug\LinqPadUiPrototype.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
</Query>

//var frm = new System.Windows.Forms.Form();

//var dlgResult = frm.ShowDialog();
//dlgResult.Dump();

var d = LinqPadUiPrototype.Class1.ShowUi();
d.Dump();