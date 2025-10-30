using System.ComponentModel.DataAnnotations;

namespace ClassHub.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O campo Login é obrigatório.", AllowEmptyStrings =false)]
        public string Login { get; set; }
        [Required(ErrorMessage = "O campo senha é obrigatório.", AllowEmptyStrings = false)]
        public string Senha { get; set; }
    }
}
