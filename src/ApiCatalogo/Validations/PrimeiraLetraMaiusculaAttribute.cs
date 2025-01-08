using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.Validations
{
    public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
    {

        //Primeira maneira de abordar e criar uma validação
        protected override ValidationResult? IsValid(object? value,
            ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            
            var primeiraLetra = value.ToString()[0].ToString();
            if (primeiraLetra != primeiraLetra.ToUpper())
            {
                return new ValidationResult(
                    "A primeira letra do nome do produto deverá ser maiúscula");
            }

            return ValidationResult.Success;
        }
    }
}
