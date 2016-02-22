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

namespace ApiDocs.Validation
{
    using ApiDocs.Validation.Error;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Csdl;

    /// <summary>
    /// Provides capabilities to validation an EntityFramework and DocSet match.
    /// </summary>
    public class EdmxValidator
    {
        /// <summary>
        /// Verifies that the resource defintions in the docs and the entity framework are consistent.
        /// This expects that the source of truth is the documentation, so errors are always in the context
        /// of the docs being right and the edmx being in error.
        /// </summary>
        /// <param name="edmx"></param>
        /// <param name="docs"></param>
        /// <returns></returns>
        public static ValidationError[] CompareResourceDefinitions(Csdl.EntityFramework edmx, DocSet docs, Json.ValidationOptions options = null)
        {
            List<ValidationError> errors = new List<ValidationError>();

            ResourceDefinition[] generatedResources = Csdl.ODataParser.GenerateResourcesFromSchemas(edmx);
            foreach (var resource in generatedResources)
            {
                // Validate this resource vs. our docs
                var matchingDocResources = (from r in docs.Resources where r.Name == resource.Name select r);

                if (!matchingDocResources.Any())
                {
                    
                    errors.Add(new ValidationError(ValidationErrorCode.ResourceTypeNotFound, null, "Undocumented resource found: {0}", resource.Name));
                    continue;
                }

                ValidationError[] detectedErrors = null;
                docs.ResourceCollection.ValidateJsonExample(resource.OriginalMetadata, resource.ExampleText, out detectedErrors, options);
                if (detectedErrors != null && detectedErrors.Any())
                {
                    errors.Add(ValidationError.NewConsolidatedError(ValidationErrorCode.ConsolidatedError, detectedErrors, "Resource {0} failed validation.", resource.Name));
                }
            }

            return errors.ToArray();
        }


        /// <summary>
        /// Validates that the JSON pathes defined in the documentation are available in the EDMX.
        /// Also finds navigation properties, actions, and functions in the EDMX that are not exercised in the documentation.
        /// </summary>
        /// <param name="edmx"></param>
        /// <param name="docs"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ValidationError[] CompareRestPathDefinitions(Csdl.EntityFramework edmx, DocSet docs, Json.ValidationOptions options = null)
        {
            var source = EntityFrameworkGenerator.Generate(docs, null, null);
            CsdlComparer comparer = new CsdlComparer();
            return comparer.CompareFrameworks(source, edmx);
        }
    }
}
