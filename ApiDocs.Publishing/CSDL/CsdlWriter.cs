/*
 * Markdown Scanner
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace ApiDocs.Publishing.CSDL
{
    using ApiDocs.Validation;
    using ApiDocs.Validation.Writers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ApiDocs.Validation.OData;

    public class CsdlWriter : DocumentPublisher
    {
        private readonly string[] validNamespaces;
        private readonly string baseUrl;

        public CsdlWriter(DocSet docs, string[] namespacesToExport, string baseUrl)
            : base(docs)
        {
            this.validNamespaces = namespacesToExport;
            this.baseUrl = baseUrl;
        }

        public override async Task PublishToFolderAsync(string outputFolder)
        {
            // Step 1: Generate an EntityFramework OM from the documentation
            EntityFramework framework = EntityFrameworkGenerator.Generate(this.Documents, this.baseUrl, this.validNamespaces);

            // Step 2: Generate XML representation of EDMX
            var xmlData = ODataParser.GenerateEdmx(framework);

            // Step 3: Write the XML to disk
            var outputDir = new System.IO.DirectoryInfo(outputFolder);
            outputDir.Create();

            var outputFilename = System.IO.Path.Combine(outputFolder, "metadata.edmx");
            using (var writer = System.IO.File.CreateText(outputFilename))
            {
                await writer.WriteAsync(xmlData);
                await writer.FlushAsync();
                writer.Close();
            }
        }
    }

}
