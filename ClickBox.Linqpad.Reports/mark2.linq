<Query Kind="Statements" />

var id = "CAT.007.002.0001.0004";
id.Dump();
var parts = id.Split('.');
var newId = string.Empty;

for (int i = 0; i < parts.Length-2; i++)
{
	newId = newId + parts[i] +".";
}

newId = newId + parts[parts.Length-1];

newId.Dump();

string asfd = "yada.pdf";

Path.GetExtension(asfd).Dump();