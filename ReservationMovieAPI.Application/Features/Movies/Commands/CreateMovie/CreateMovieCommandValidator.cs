using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Movies.Commands.CreateMovie
{
    public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
    {
        public CreateMovieCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Film başlığı zorunludur.")
                .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Film açıklaması zorunludur.")
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("Süre 0'dan büyük olmalıdır.")
                .LessThan(600).WithMessage("Süre 600 dakikadan fazla olamaz.");

            RuleFor(x => x.ReleaseDate)
                .NotEmpty().WithMessage("Vizyon tarihi zorunludur.");

            RuleFor(x => x.Director)
                .NotEmpty().WithMessage("Yönetmen bilgisi zorunludur.")
                .MaximumLength(100).WithMessage("Yönetmen adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("Dil bilgisi zorunludur.")
                .MaximumLength(10);

            RuleFor(x => x.AgeRestriction)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(18);

            RuleFor(x => x.ImdbRating)
                .InclusiveBetween(0, 10)
                .When(x => x.ImdbRating.HasValue)
                .WithMessage("IMDB puanı 0-10 arasında olmalıdır.");

            RuleFor(x => x.GenreIds)
                .NotEmpty().WithMessage("En az bir tür seçilmelidir.");

            RuleFor(x => x.TrailerUrl)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.TrailerUrl))
                .WithMessage("Geçerli bir URL giriniz.");
        }
    }
}
