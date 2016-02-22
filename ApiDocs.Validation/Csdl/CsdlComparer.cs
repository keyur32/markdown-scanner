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

namespace ApiDocs.Validation.Csdl
{
    using ApiDocs.Validation.Error;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// Contains functionality to compare to EntityFramework instances and report back
    /// the detected differences
    /// </summary>
    public class CsdlComparer
    {
        /// <summary>
        /// Enables converting between namespaces (e.g. oneDrive -> microsoft.graph)
        /// </summary>
        public Dictionary<string, string> NamespaceConversionTable { get; set; }

        public CsdlComparer()
        {
            this.NamespaceConversionTable = new Dictionary<string, string>();
        }

        /// <summary>
        /// Compare two entity framework instances and report on their differences
        /// </summary>
        /// <param name="source">This is assumed to be the "truth". Differeces are reported in terms of this being the truth.</param>
        /// <param name="remote">An entity framework to compare to the source.</param>
        /// <returns></returns>
        public ValidationError[] CompareFrameworks(EntityFramework source, EntityFramework remote)
        {
            List<ValidationError> errors = new List<ValidationError>();

            // Find any extra schemas that remote has that source doesn't.
            var remoteSchemas = from r in remote.DataServices.Schemas
                               select new { Remote = r, Source = FindMatchingSchema(r.Namespace, source.DataServices.Schemas) };
            foreach (var schema in remoteSchemas)
            {
                if (null == schema.Source)
                {
                    errors.Add(new ValidationError(ValidationErrorCode.ResourceTypeNotFound, "remote", "Remote schema namespace '{0}' not found in source.", schema.Remote.Namespace));
                }
                else
                {
                    ValidationError[] detectedErrors = CompareSchemas(schema.Source, schema.Remote);

                    if (detectedErrors.Any())
                    {
                        errors.Add(ValidationError.NewConsolidatedError(ValidationErrorCode.ConsolidatedError, detectedErrors, "Schema {0} had changes from the source.", schema));
                    }
                }
            }
            return errors.ToArray();
        }

        /// <summary>
        /// Compares the members of a schema and reports on any differences between source and remote.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        private ValidationError[] CompareSchemas(Schema source, Schema remote)
        {
            // Compare the members of source and remote schemas
            List<ValidationError> errors = new List<ValidationError>();



            return errors.ToArray();
        }

 

        #region Namespace resolution helpers
        private Schema FindMatchingSchema(string remoteNamespace, IEnumerable<Schema> sourceSchemas)
        {
            foreach (var schema in sourceSchemas)
            {
                if (EquivelentNamespaces(remoteNamespace, schema.Namespace))
                    return schema;
            }
            return null;
        }

        private bool EquivelentNamespaces(string ns1, string ns2)
        {
            if (ns1 == ns2)
                return true;

            if (NamespaceConversionTable.ContainsKey(ns1))
            {
                if (NamespaceConversionTable[ns1] == ns2)
                    return true;
            }

            if (NamespaceConversionTable.ContainsKey(ns2))
            {
                if (NamespaceConversionTable[ns2] == ns1)
                    return true;
            }

            return false;
        }
        #endregion




    }
}
