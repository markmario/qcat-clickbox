<Query Kind="Program">
  <Reference Relative="..\..\exportmdb.interop\build\Release\ExportMdb.Interop.dll">C:\lw\exportmdb.interop\build\Release\ExportMdb.Interop.dll</Reference>
  <Namespace>ExportMdb.Interop.Core</Namespace>
  <Namespace>ExportMdb.Interop.Core.Comparers</Namespace>
  <Namespace>ExportMdb.Interop.Core.Helpers</Namespace>
  <Namespace>ExportMdb.Interop.Legal</Namespace>
</Query>

       static void Main(string[] args)
        {
            var mdb = new ExportMdb.Interop.Legal.ExportMdb(@"C:\Users\Simon\Desktop\Rindexer\Test Data\export.mdb");
            mdb.Load(new ExportMdb.Interop.Core.LoadOptions() {BuildHostAttachmentTree = true, ChunkSize = 100, DataTablesToLoad =  ExportMdbData.All});
            var imagesToCopy = new List<Tuple<string, string>>();
            
            mdb.Documents.AsParallel().ForAll(f =>
            {
                var newId = GetNewDocId(f.DocumentId);
                f.Pages.AsParallel().ForAll(p =>
                {
				var oldImage = GetDocumentRelativePath(p.DocumentId, p.ImageFileName);
				if (p.PageLabel == "PDF")
                    {
                        p.ImageFileName = newId + Path.GetExtension(p.ImageFileName);
                    }
                    imagesToCopy.Add(Tuple.Create(oldImage,
                        GetDocumentRelativePath(newId, p.ImageFileName)));
                    
                });
                f.UpdateDocumentId(newId); 
            });

            imagesToCopy.AsParallel().ForAll(CopyToFiles);
            var newMdb = new ExportMdb.Interop.Legal.ExportMdb(@"C:\Users\Simon\Desktop\Rindexer\Test Data\export2.mdb");
            newMdb.Documents = mdb.Documents;
            newMdb.Save(ExportMdbData.All,true);
        }

        private static void CopyToFiles(Tuple<string, string> item)
        {
            var oldFile = @"C:\Users\Simon\Desktop\Rindexer\Test Data" +item.Item1;

            var newFile = @"C:\Users\Simon\Desktop\Rindexer\Test Data\Updated" + item.Item2;

            var newDir = Path.GetDirectoryName(newFile);
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            File.Copy(oldFile, newFile);
        }

        public static string GetDocumentRelativePath(string documentId, string fileName)
        {
            string[] strArray = documentId.Split('.');
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("\\");
            for (int index = 0; index < strArray.Length - 1; ++index)
            {
                stringBuilder.AppendFormat("{0}\\", strArray[index]);
            }
            stringBuilder.Append(fileName);
            return stringBuilder.ToString();
        }

        private static string GetNewDocId(string documentId)
        {
            var parts = documentId.Split('.');
            var newId = string.Empty;

            for (int i = 0; i < parts.Length - 2; i++)
            {
                newId = newId + parts[i] + ".";
            }

            newId = newId + parts[parts.Length - 1];

            return newId;
        }