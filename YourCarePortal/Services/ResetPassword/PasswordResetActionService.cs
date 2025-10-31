using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using YourCarePortal.Models; // Ensure this is the correct namespace for your models
using YourCarePortal.Services;

public class PasswordResetActionService
{
    private readonly PasswordResetService _passwordResetService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PasswordResetActionService(PasswordResetService passwordResetService, IHttpContextAccessor httpContextAccessor)
    {
        _passwordResetService = passwordResetService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(object model, bool success)> ProcessResetPasswordAction(string currentStep, string tempCode, string email, string newPassword,string tempCode_hidden)
    {
        RootIndexResetPassword forgotPasswordSequence = null;
        object model;

        switch (currentStep)
        {
            case "1":
                forgotPasswordSequence = await _passwordResetService.StartPasswordResetSequence(email, currentStep);

                if (forgotPasswordSequence?.status == "OK")
                {

                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "2",
                        tempCode_hidden = forgotPasswordSequence.tempCode_hidden,
                        email = email
                    };

                    return (model, true);
                }
                else
                {
                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "1",
                        ErrorMessage = forgotPasswordSequence.message,
                        email = email

                    };

                    return (model, true);
                }

            case "2":

                forgotPasswordSequence = await _passwordResetService.ValidateTemporaryCode(email, tempCode, tempCode_hidden);
                if (forgotPasswordSequence?.status == "OK")
                {
                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "3",
                        //ErrorMessage = forgotPasswordSequence.message,
                        tempCode = tempCode,
                        tempCode_hidden = tempCode_hidden = tempCode_hidden,
                        email = email,
                        newPassword = newPassword
                    };

                    return (model, true);
                }
                else
                {
                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "2",
                        ErrorMessage = forgotPasswordSequence.message,
                        tempCode = tempCode,
                        tempCode_hidden = tempCode_hidden,
                        email = email,
                        newPassword = newPassword
                    };

                    return (model, true);
                }

            case "3":

                forgotPasswordSequence = await _passwordResetService.FinalizePasswordReset(email, tempCode, newPassword, tempCode_hidden);
                if (forgotPasswordSequence?.status == "OK")
                {
                 ;
                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "4",
                        tempCode = tempCode,
                        tempCode_hidden = tempCode_hidden,
                        email = email
                    };

                    return (model, true);
                }
                else
                {
                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "3",
                        ErrorMessage = forgotPasswordSequence.message,
                        email = email,
                        newPassword = newPassword
                    };
                    return (model, true);
                }

            case "4":

                forgotPasswordSequence = await _passwordResetService.FinalizePasswordReset(email, tempCode, newPassword, tempCode_hidden);
                
                if (forgotPasswordSequence?.status == "OK")
                {

                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "4",
                        //ErrorMessage = forgotPasswordSequence.message,
                        tempCode_hidden = tempCode_hidden,
                        email = email
                    };

                    return (model, true);
                }
                else
                {
                    model = new RootIndexResetPassword
                    {
                        CurrentStep = "3",
                        ErrorMessage = forgotPasswordSequence.message,
                        email = email,
                        newPassword = newPassword
                    };

                    return (model, true);
                }

            default:
                // Log error or handle invalid step
                return (null, false); // Or some error handling mechanism
        }
    }

    // Other methods if needed...
}
