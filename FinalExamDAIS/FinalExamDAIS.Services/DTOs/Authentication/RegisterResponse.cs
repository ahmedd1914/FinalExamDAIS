using FinalExamDAIS.Services.DTOs;

namespace FinalExamDAIS.Services.DTOs.Authentication
{
    public class RegisterResponse : BaseResponse<UserInfo>
    {
        public UserInfo UserInfo 
        { 
            get => Data; 
            set => Data = value; 
        }
    }
}
