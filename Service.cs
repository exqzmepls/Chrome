using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using System.IO;
using static Google.Apis.Drive.v3.DriveService;

namespace Chrome
{
    internal class Service
    {
        private readonly DriveService _driveService;
        public Service(ServiceSettings settings)
        {
            var tokenResponse = new TokenResponse
            {
                AccessToken = settings.AccessToken,
                RefreshToken = settings.RefreshToken,
            };

            var codeFlowInitializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = settings.ClientId,
                    ClientSecret = settings.ClientSecret
                },
                Scopes = new[] { Scope.Drive },
                DataStore = new FileDataStore(settings.ApplicationName)
            };

            var apiCodeFlow = new GoogleAuthorizationCodeFlow(codeFlowInitializer);

            var credential = new UserCredential(apiCodeFlow, settings.UserName, tokenResponse);
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = settings.ApplicationName
            });

            _driveService = service;
        }

        public UploadStatus UploadPhoto(Stream fileStream, string fileName)
        {
            var driveFile = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
            };

            var request = _driveService.Files.Create(driveFile, fileStream, "image/jpeg");

            var response = request.Upload();

            return response.Status;
        }
    }
}
