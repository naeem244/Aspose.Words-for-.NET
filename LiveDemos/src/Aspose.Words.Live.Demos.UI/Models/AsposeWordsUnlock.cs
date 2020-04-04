using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Aspose.Words.Live.Demos.UI.Models;
using Aspose.Words.Live.Demos.UI.Models.Common;
using Aspose.Words.Live.Demos.UI.Services;
using Aspose.Words;

namespace Aspose.Words.Live.Demos.UI.Models
{
	///<Summary>
	/// AsposeWordsUnlock class to remove password in documents
	///</Summary>
	public class AsposeWordsUnlock : AsposeWordsBase
	{
		///<Summary>
		/// Unlock method
		///</Summary>
		public Response Unlock( InputFiles files, string outputType, string passw)
		{
			
			if (files.Count == 0 || files.Count > MaximumUploadFiles)
				return MaximumFileLimitsResponse;

			SetDefaultOptions(files, outputType);
			Opts.AppName = "Unlock";
			Opts.MethodName = "Unlock";
			Opts.ZipFileName = "Unlocked documents";

			var lck = new object();
			var catchedException = false;
			var strError = new StringBuilder();
			var docs = new Document[files.Count];
			for (var i = 0; i < files.Count; i++)
			{
				try
				{
					docs[i] = new Document(files[i].FullFileName, new LoadOptions(passw));
				}
				catch (IncorrectPasswordException ex)
				{
					lock (lck)
					{
						strError.Append($"File \"{files[i].FileName}\" - {ex.Message}");
						catchedException = true;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					lock (lck)
						catchedException = true;
					
				}
			}

			if (!catchedException)
			{
				return  Process((inFilePath, outPath, zipOutFolder) =>
				{
					var tasks = docs.Select(doc => Task.Factory.StartNew(() =>
					{
						SaveDocument(doc, outPath, zipOutFolder);
					})).ToArray();
					Task.WaitAll(tasks);
				});
			}

			return new Response
			{
				Status = strError.Length > 0 ? strError.ToString() : "Exception during processing",
				StatusCode = 500
			};
		}
	}
}