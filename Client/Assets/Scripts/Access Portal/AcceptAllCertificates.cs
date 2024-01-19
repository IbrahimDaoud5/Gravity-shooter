using UnityEngine.Networking;

// TEMPORARY - until we use CA
// TEMPORARY - until we use CA
// TEMPORARY - until we use CA
// TEMPORARY - until we use CA
// TEMPORARY - until we use CA


// ****************************************************** THIS CLASS IS TEMPORARY *****************************
// ****************************************************** AND SHOULD BE DELETED WHEN WE USE CA ****************
// ****************************************************** BEFORE PRODUCTION ***********************************
public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // WARNING: This is for development purposes only, do NOT use in production!
        return true; // Accept all certificates
    }
}

// TEMPORARY - until we use CA
// TEMPORARY - until we use CA
// TEMPORARY - until we use CA
// TEMPORARY - until we use CA
// TEMPORARY - until we use CA
