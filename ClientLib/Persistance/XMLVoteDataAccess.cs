using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ClientLib.Persistance
{
    public class XMLVoteDataAccess : ILocalVoteDataAccess
    {
        private string XmlFolder
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\FOOVotingApp";
            }
        }

        private const string xmlFileName = "localstorage.xml";

        private string XmlPath
        {
            get { return XmlFolder + "\\" + xmlFileName; }
        }

        public void StoreVote(string username, SignedBallotModel ballot, BigInteger transportEncryptedBallot, RSA pollKeys, int optionId)
        {
            var doc = new XmlDocument();
            if (!Directory.Exists(XmlFolder))
            {
                Directory.CreateDirectory(XmlFolder);
            }
            if (!File.Exists(XmlPath))
            {
                var xmlRoot = new XmlDocument();
                xmlRoot.AppendChild(xmlRoot.CreateElement("Polls"));
                xmlRoot.Save(XmlPath);
            }
            doc.Load(XmlPath);

            XmlElement pollElement = doc.CreateElement("Poll");
            pollElement.SetAttribute("Id", ballot.PollId.ToString());
            pollElement.SetAttribute("User", username.ToLower());
            pollElement.SetAttribute("Validated", "false");

            var keysElement = doc.CreateElement("Keys");
            keysElement.InnerText = pollKeys.ToXmlString(true);
            pollElement.AppendChild(keysElement);

            var cb = doc.CreateElement("CommitedBallot");
            cb.InnerText = new BigInteger(ballot.Ballot).ToString();
            pollElement.AppendChild(cb);

            var transEncBallot = doc.CreateElement("TransportEncryptedBallot");
            transEncBallot.InnerText = transportEncryptedBallot.ToString();
            pollElement.AppendChild(transEncBallot);

            var scb = doc.CreateElement("VerificationSignature");
            scb.InnerText = new BigInteger(ballot.Signature!).ToString();
            pollElement.AppendChild(scb);

            var votedOptionId = doc.CreateElement("VotedOptionId");
            votedOptionId.InnerText = optionId.ToString();
            pollElement.AppendChild(votedOptionId);

            var root = doc.SelectSingleNode("Polls");
            root!.AppendChild(pollElement);

            doc.Save(XmlPath);
        }

        public bool TryGetVotedOption(string username, int pollId, out int optionId)
        {
            optionId = 0;
            if (!File.Exists(XmlPath))
            {
                return false;
            }

            var xml = new XmlDocument();
            xml.Load(XmlPath);
            var polls = xml.SelectNodes("//Poll");
            if (polls == null)
            {
                return false;
            }

            foreach (XmlElement p in polls)
            {
                if (int.Parse(p.GetAttribute("Id")) == pollId && p.GetAttribute("User") == username.ToLower())
                {
                    var votedOptionNode = ((XmlNode)p).SelectSingleNode("VotedOptionId");
                    if (votedOptionNode == null)
                    {
                        return false;
                    }
                    optionId = int.Parse(votedOptionNode.InnerText);
                    return true;
                }
            }

            return false;
        }

        public bool TryGetBallot(string username, int pollId, out SignedBallotModel? ballot)
        {
            ballot = null;
            if (!File.Exists(XmlPath))
            {
                return false;
            }

            var xml = new XmlDocument();
            xml.Load(XmlPath);
            var polls = xml.SelectNodes("//Poll");
            if (polls == null)
            {
                return false;
            }

            foreach (XmlElement p in polls)
            {
                if (int.Parse(p.GetAttribute("Id")) == pollId && p.GetAttribute("User") == username.ToLower())
                {

                    var ballotNode = ((XmlNode)p).SelectSingleNode("CommitedBallot");
                    if (ballotNode == null)
                    {
                        return false;
                    }
                    ballot = new SignedBallotModel { Ballot = BigInteger.Parse(ballotNode.InnerText).ToByteArray(), PollId = pollId };

                    var signatureNode = ((XmlNode)p).SelectSingleNode("VerificationSignature");
                    if (signatureNode == null)
                    {
                        return false;
                    }
                    ballot.Signature = BigInteger.Parse(signatureNode.InnerText).ToByteArray();
                    return true;
                }
            }

            return false;
        }

        public bool TryGetTransportEncryptedBallot(string username, int pollId, out BigInteger encBallot)
        {
            encBallot = 0;
            if (!File.Exists(XmlPath))
            {
                return false;
            }

            var xml = new XmlDocument();
            xml.Load(XmlPath);
            var polls = xml.SelectNodes("//Poll");
            if (polls == null)
            {
                return false;
            }

            foreach (XmlElement p in polls)
            {
                if (int.Parse(p.GetAttribute("Id")) == pollId && p.GetAttribute("User") == username.ToLower())
                {

                    var ballotNode = ((XmlNode)p).SelectSingleNode("TransportEncryptedBallot");
                    if (ballotNode == null)
                    {
                        return false;
                    }
                    encBallot = BigInteger.Parse(ballotNode.InnerText);

                    return true;
                }
            }

            return false;
        }

        public bool TryGetKey(string username, int pollId, out RSA? key)
        {
            key = null;
            if (!File.Exists(XmlPath))
            {
                return false;
            }

            var xml = new XmlDocument();
            xml.Load(XmlPath);
            var polls = xml.SelectNodes("//Poll");
            if (polls == null)
            {
                return false;
            }

            foreach (XmlElement p in polls)
            {
                if (int.Parse(p.GetAttribute("Id")) == pollId && p.GetAttribute("User") == username.ToLower())
                {
                    var keysNode = ((XmlNode)p).SelectSingleNode("Keys");
                    if (keysNode == null)
                    {
                        return false;
                    }

                    RSA ret = new RSACryptoServiceProvider();
                    ret.FromXmlString(keysNode.InnerText);
                    key = ret;

                    return true;
                }
            }

            return false;
        }

        public ValidatedState GetValidatedState(string username, int pollId)
        {
            if (!File.Exists(XmlPath))
            {
                return ValidatedState.DifferentMachine;
            }

            var xml = new XmlDocument();
            xml.Load(XmlPath);
            var polls = xml.SelectNodes("//Poll");
            if (polls == null)
            {
                return ValidatedState.DifferentMachine;
            }

            foreach (XmlElement p in polls)
            {
                if (int.Parse(p.GetAttribute("Id")) == pollId && p.GetAttribute("User") == username.ToLower())
                {
                    if (p.GetAttribute("Validated") == "true")
                    {
                        return ValidatedState.Yes;
                    }
                    else
                    {
                        return ValidatedState.No;
                    }
                }
            }

            return ValidatedState.DifferentMachine;
        }

        public void UpdateValidatedAttribute(string username, int pollId)
        {
            if (!File.Exists(XmlPath))
            {
                return;
            }

            var xml = new XmlDocument();
            xml.Load(XmlPath);
            var polls = xml.SelectNodes("//Poll");
            if (polls == null)
            {
                return;
            }

            bool found = false;
            foreach (XmlElement p in polls)
            {
                if (int.Parse(p.GetAttribute("Id")) == pollId && p.GetAttribute("User") == username.ToLower())
                {
                    p.SetAttribute("Validated", "true");
                    found = true;
                }
            }

            if (found)
            {
                xml.Save(XmlPath);
            }
        }
    }
}
