﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface
{
    public interface ICampusIdentifier
    {
        long MasterUKPRN { get; set; }

        string CampusIdentifer { get; set; }
    }
}
