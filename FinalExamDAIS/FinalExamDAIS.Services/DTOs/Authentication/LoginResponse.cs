using FinalExamDAIS.Services.DTOs;

namespace FinalExamDAIS.Services.DTOs.Authentication
{
    public class LoginResponse : BaseResponse<UserInfo>
    {
        public UserInfo UserInfo 
        { 
            get => Data; 
            set => Data = value; 
        }
    }
}