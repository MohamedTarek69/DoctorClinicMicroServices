using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.PatientDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Services
{
    public class PatientClient : IPatientClient
    {
        private readonly HttpClient _httpClient;

        public PatientClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<ReturnedPatientDetailsDto>>GetPatientDetailsByIdentityUserIdAsync(Guid identityUserId, string token){
            var httpRequest =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    $"Patiant/DetailsByIdentityUserId/{identityUserId}");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token.Replace("Bearer ", ""));

            var response =
                await _httpClient.SendAsync(httpRequest);

            var content =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Result<ReturnedPatientDetailsDto>.Fail(
                    Error.Failure(
                        "Patient.GetFailed",
                        content));
            }

            var result =
                JsonSerializer.Deserialize<
                    ReturnedPatientDetailsDto>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return Result<ReturnedPatientDetailsDto>
                .Ok(result!);
        }
    }
}
