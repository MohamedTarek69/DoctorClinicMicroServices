using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Services
{
    using ClinicMicroServices.Services_Abstraction.Interfaces;
    using ClinicMicroServices.Shared.CommonResult;
    using ClinicMicroServices.Shared.DTOs.DoctorDtos;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text.Json;
    using static System.Net.WebRequestMethods;

    public class IdentityClient : IIdentityClient
    {
        private readonly HttpClient _httpClient;

        public IdentityClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<string>> RegisterDoctorAsync(CreateDoctorRequest request)
        {
            var payload = new
            {
                displayName = request.DisplayName,
                email = request.Email,
                password = request.Password,
                phoneNumber = request.PhoneNumber, // ✅ غالبًا Required
                role = "Doctor"
            };

            var response = await _httpClient.PostAsJsonAsync("/Clinic/Authentication/Register", payload);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();

                // ✅ رجّع السبب الحقيقي للفرونت/السواغر
                return Result<string>.Fail(Error.Validation("Identity.RegisterFailed", body));
            }

            // ⚠️ انت قلت قبل كده Identity بيرجع displayName/email بس
            // يبقى السطر ده هيفشل لو مفيش UserId
            var result = await response.Content.ReadFromJsonAsync<IdentityRegisterResponse>();

            if (result is null)
                return Result<string>.Fail(Error.Failure("Identity.InvalidResponse", "Identity returned empty response"));

            return Result<string>.Ok(result.Id);
        }

        public async Task<Result<UpdateIdentityUserResponse>> UpdateDoctorAsync(
                                                                 string userId,
                                                                 UpdateIdentityUserRequest request,
                                                                 string token) // 👈 مهم
        {
            var httpRequest = new HttpRequestMessage(
                HttpMethod.Patch,
                $"/Clinic/Authentication/UpdateUser/{userId}");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Result<UpdateIdentityUserResponse>.Fail(
                    Error.Failure("Identity.UpdateFailed", content)
                );
            }

            var result = JsonSerializer.Deserialize<UpdateIdentityUserResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Result<UpdateIdentityUserResponse>.Ok(result!);
        }

        public async Task<Result<bool>> UpdatePasswordAsync(
                                            string userId,
                                            UpdateDoctorPasswordRequest newPassword,
                                            string token) // 👈 مهم
        {
            var payload = new { newPassword = newPassword.NewPassword };

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Patch,
                $"/Clinic/Authentication/UpdatePassword/{userId}");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            httpRequest.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(httpRequest);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Result<bool>.Fail(
                    Error.Failure("Identity.PasswordUpdateFailed", content)
                );
            }

            return Result<bool>.Ok(true);
        }

        public async Task<bool> IsDoctorActiveAsync(string identityUserId)
        {
            var res = await _httpClient.GetAsync($"/doctors/internal/is-active/{identityUserId}");
            if (!res.IsSuccessStatusCode) return false;

            var body = await res.Content.ReadFromJsonAsync<IsActiveResponse>();
            return body?.IsActive ?? false;
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        }

    }
}
