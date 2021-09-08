using Helpers.Cineworld.Models;
using static Dawn.Guard;

namespace Dawn;

public static class DawnGuardExtensions
{
	private const string _postcodePattern = @"\w{1,2}\d{1,2}\w?\s?\d\w{2}";

	public static ref readonly ArgumentInfo<short> IsCinemaId(in this ArgumentInfo<short> argument)
	{
		var message = argument.Name + " must be positive";
		return ref argument.Positive(_ => message);
	}

	public static ref readonly ArgumentInfo<int> IsFilmEdi(in this ArgumentInfo<int> argument)
	{
		var message = argument.Name + " must be positive";
		return ref argument.Positive(_ => message);
	}

	public static ref readonly ArgumentInfo<string> IsPostcode(in this ArgumentInfo<string> argument)
	{
		var message = $"{argument.Name} must match {_postcodePattern}";
		return ref argument
			.NotNull()
			.NotEmpty()
			.NotWhiteSpace()
			.Matches(_postcodePattern, (_, _) => message);
	}

	public static ref readonly ArgumentInfo<Cinema> IsCinema(in this ArgumentInfo<Cinema> argument)
	{
		argument.NotNull().Wrap(c => c.Id).IsCinemaId();
		argument.Wrap(c => c.Name).NotNull().NotEmpty().NotWhiteSpace();
		argument.Wrap(c => c.Postcode).IsPostcode();
		return ref argument;
	}

	public static ref readonly ArgumentInfo<Film> IsFilm(in this ArgumentInfo<Film> argument)
	{
		argument.NotNull().Wrap(f => f.Edi).IsFilmEdi();
		argument.Wrap(f => f.Title).NotNull().NotEmpty().NotWhiteSpace();
		argument.Wrap(f => f.Length).Positive();
		return ref argument;
	}

	public static ref readonly ArgumentInfo<Show> IsShow(in this ArgumentInfo<Show> argument)
	{
		argument.NotNull().Wrap(s => s.CinemaId).IsCinemaId();
		argument.NotNull().Wrap(s => s.FilmEdi).IsFilmEdi();
		argument.Wrap(f => f.DateTime).NotDefault();
		return ref argument;
	}
}
