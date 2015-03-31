﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneDrive.ApiDocumentation.Validation
{
    public static class ValidationConfig
    {
        static ValidationConfig()
        {
            ValidationConfig.ExpectedResponseAsRequiredProperties = true;
            ValidationConfig.AdditionalHttpHeaders = new string[0];
        }

        /// <summary>
        /// Validatation requires that properties shown in the documentation's expected response are
        /// found when testing the service or simulatedResponse.
        /// </summary>
        public static bool ExpectedResponseAsRequiredProperties { get; set; }

        /// <summary>
        /// Instead of using the default OData metadata settings, force the odata metadata parameters to none.
        /// </summary>
        public static string ODataMetadataLevel { get; set; }

        /// <summary>
        /// An array of additional HTTP headers that are added to outgoing requests to the service.
        /// </summary>
        public static string[] AdditionalHttpHeaders { get; set; }


    }
}