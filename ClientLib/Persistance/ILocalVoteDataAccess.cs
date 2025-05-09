﻿using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Persistance
{
    public interface ILocalVoteDataAccess
    {
        void StoreVote(string username, SignedBallotModel ballot, BigInteger transportEncryptedBallot, RSA pollKeys, int optionId);
        bool TryGetVotedOption(string username, int pollId, out int optionId);
        bool TryGetBallot(string username, int pollId, out SignedBallotModel? ballot);
        bool TryGetTransportEncryptedBallot(string username, int pollId, out BigInteger encBallot);
        bool TryGetKey(string username, int pollId, out RSA? key);
        ValidatedState GetValidatedState(string username, int pollId);
        void UpdateValidatedAttribute(string username, int pollId);
    }
}
