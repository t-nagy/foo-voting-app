using System.Security.Cryptography;

namespace AdminAPI.Services
{
    public class KeyService
    {
        public RSACryptoServiceProvider RSA { get; }

        public KeyService()
        {
            RSA = new RSACryptoServiceProvider();
            RSA.ImportFromPem("-----BEGIN RSA PRIVATE KEY-----\r\nMIICXAIBAAKBgQDM7ljYGYSgGaqPiCnctbBBF6bt3sWZvsccqR0XtN1GiAZTREIs\r\n7Knc3muG8k7qn792swpvYO7dZrn2a1MAweKwZGGTQ45h/zqv0DFx2F6NBqtg7AGO\r\n1yUjN5dsr31s+HGB8adfjQpp3XBD/6f39Cv/5LX17sSUx9UoYL5v4sl7BQIDAQAB\r\nAoGAZcUP9fFmPaPBYho6v9KyvwCh84soElv9wavyOK2nHbm7empxeqHlETybpZ57\r\ntmSyzp3HVtyKgwTa4RcXV07x5cennQ1itz+y8I0pnKmJAqBpMemabAVZDvr804j+\r\nSs9cw8Grrpw4hnloAOCmoZNCxknTKi6q+IFeKYU+vRnEylkCQQDcJ2RQ1N9ueYb4\r\nSR5yGvx4RgSt65gFanZsyjTEv2I5EZ2074lei3LpzHfwIOCCDK/mQKmoOVQN5Vg7\r\nmVaCXTfHAkEA7kxtLKZwOBsINx0fUaR0RoEHlaku53Y1Y56mGjS080K7bRDP79wP\r\nBYXbkRzSrxRV222i+T7h7NbC62RqgBtu0wJBAM5bhiZWQtCUzTSxpP4j2X8Lcptr\r\nd+WrszGqH+hD1FfV8VOGK+cZIy+PXuUQjCCar85N0jlC80zLKvdCddpgckUCQErR\r\n4kFvrLJhAm5TjX7T1NJCNwBtk1WHTvINYPe/bsUmQbTX34HfJTRFuA/S7e+cwexY\r\neyAo90SmlaLU739znZMCQFbeERSIJXVsUnGvSZLuzE5M+be5OYOfyIqpdJs7SdCB\r\ngA0KWC1OGNQqk/mi/1XrRVE5c5HTuTv8yYS40buR6Qo=\r\n-----END RSA PRIVATE KEY-----");
        }
    }
}
