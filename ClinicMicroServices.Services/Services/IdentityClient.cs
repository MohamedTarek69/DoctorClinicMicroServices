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
    using System.Net.Http.Json;
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

        public async Task<Result<UpdateIdentityUserResponse>> UpdateDoctorAsync(string userId, UpdateIdentityUserRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync($"/Clinic/Authentication/UpdateUser/{userId}", request);

            if (!response.IsSuccessStatusCode)
                return Result<UpdateIdentityUserResponse>.Fail(
                    Error.Failure("Identity.UpdateFailed", "Failed to update identity user")
                );

            var result = await response.Content.ReadFromJsonAsync<UpdateIdentityUserResponse>();
            return Result<UpdateIdentityUserResponse>.Ok(result!);
        }

        public async Task<Result<bool>> UpdatePasswordAsync(string userId, UpdateDoctorPasswordRequest newPassword)
        {
            var payload = new { newPassword = newPassword.NewPassword };
            var response = await _httpClient.PatchAsJsonAsync($"/Clinic/Authentication/UpdatePassword/{userId}", payload);
            if (!response.IsSuccessStatusCode)
                return Result<bool>.Fail(
                    Error.Failure("Identity.PasswordUpdateFailed", "Failed to update password")
                );
            return Result<bool>.Ok(true);
        }

        public async Task<bool> IsDoctorActiveAsync(string identityUserId)
        {
            var res = await _httpClient.GetAsync($"/doctors/internal/is-active/{identityUserId}");
            if (!res.IsSuccessStatusCode) return false;

            var body = await res.Content.ReadFromJsonAsync<IsActiveResponse>();
            return body?.IsActive ?? false;
        }

    }
}
