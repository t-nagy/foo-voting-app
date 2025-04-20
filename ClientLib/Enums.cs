using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib
{
    public enum VoteSubmitResult
    {
        Success,
        UnknownFailure,
        KeyRegistrationFailed,
        NoAdminVerificationKey,
        AdminRefusedToSign,
        AdminSignatureInvalid,
        ShufflerPostFailed
    }

    public enum VoteValidationResult
    {
        Success,
        UnknownFailure,
        ShufflerPostFailed
    }

    public enum ValidatedState
    {
        Yes,
        No,
        DifferentMachine
    }
}
