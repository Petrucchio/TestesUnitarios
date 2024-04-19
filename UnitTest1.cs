using AnimesProtech.Controllers;
using AnimesProtech.Models;
using AnimesProtech.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestesUnitarios
{
    [TestClass]
    public class AnimeControllerTests
    {
        private AnimeController _animeController;
        private Mock<IAnimeService> _mockService;
        private Mock<IAuthService> _mockAuthService;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IAnimeService>();
            _mockAuthService = new Mock<IAuthService>();
            _animeController = new AnimeController(_mockService.Object, _mockAuthService.Object);
        }

        [TestMethod]
        public async Task CadastrarAnime_DeveRetornarCreatedAtAction()
        {
            ConfigureAuthentication();

            var anime = new Anime { Nome = "Teste", Resumo = "Resumo do teste", Diretor = "Diretor do teste" };

            _mockService.Setup(service => service.CadastrarAnime(It.IsAny<Anime>())).ReturnsAsync(anime);

            var result = await _animeController.CadastrarAnime(anime);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(AnimeController.GetAnime), createdAtActionResult.ActionName);

            Assert.IsTrue(createdAtActionResult.RouteValues.ContainsKey("id"));
            Assert.AreEqual(anime.Id, createdAtActionResult.RouteValues["id"]);

            Assert.AreEqual(anime, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task CadastrarAnime_DeveRetornarBadRequestQuandoAnimeForInvalido()
        {
            var anime = new Anime { Nome = null, Resumo = "Resumo do teste", Diretor = "Diretor do teste" };
            _animeController.ModelState.AddModelError("Nome", "O campo Nome é obrigatório");
            var result = await _animeController.CadastrarAnime(anime);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }


        [TestMethod]
        public async Task GetAnime_DeveRetornarOkQuandoAnimeExistir()
        {
            ConfigureAuthentication();
            var anime = new Anime { Id = 1, Nome = "Teste", Resumo = "Resumo do teste", Diretor = "Diretor do teste" };
            _mockService.Setup(service => service.GetAnime(1)).ReturnsAsync(anime);
            var result = await _animeController.GetAnime(1);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(anime, okResult.Value);
        }

        [TestMethod]
        public async Task GetAnimes_DeveRetornarPaginacaoCorreta()
        {
            ConfigureAuthentication();
            var animes = new List<Anime>
    {
        new Anime { Id = 1, Nome = "Anime1", Resumo = "Resumo do Anime1", Diretor = "Diretor1" },
        new Anime { Id = 2, Nome = "Anime2", Resumo = "Resumo do Anime2", Diretor = "Diretor2" }
    };

            _mockService.Setup(service => service.GetAnimes(null, null, null, 1, 2)).ReturnsAsync(animes);
            var result = await _animeController.GetAnimes(null, null, null, 1, 2);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, (okResult.Value as List<Anime>).Count); 
        }


        [TestMethod]
        public async Task GetAnime_DeveRetornarNotFoundQuandoAnimeNaoExistir()
        {
            ConfigureAuthentication();
            _mockService.Setup(service => service.GetAnime(1)).ReturnsAsync((Anime)null);
            var result = await _animeController.GetAnime(1);
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public async Task GetAnimes_DeveRetornarOkComListaDeAnimesQuandoExistirem()
        {
            ConfigureAuthentication();
            var animes = new List<Anime>
            {
                new Anime { Id = 1, Nome = "Anime1", Resumo = "Resumo do Anime1", Diretor = "Diretor1" },
                new Anime { Id = 2, Nome = "Anime2", Resumo = "Resumo do Anime2", Diretor = "Diretor2" }
            };
            _mockService.Setup(service => service.GetAnimes(null, null, null, 1, 10)).ReturnsAsync(animes);
            var result = await _animeController.GetAnimes(null, null, null, 1, 10);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(animes, okResult.Value);
        }

        [TestMethod]
        public async Task AtualizarAnime_DeveRetornarOkQuandoAnimeAtualizado()
        {
            var anime = new Anime { Id = 1, Nome = "Anime1", Resumo = "Resumo do Anime1", Diretor = "Diretor1" };
            var animeAtualizado = new Anime { Id = 1, Nome = "AnimeAtualizado", Resumo = "Resumo do AnimeAtualizado", Diretor = "DiretorAtualizado" };
            _mockService.Setup(service => service.AtualizarAnime(1, anime)).ReturnsAsync(animeAtualizado);
            var result = await _animeController.AtualizarAnime(1, anime);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(animeAtualizado, okResult.Value);
        }

        [TestMethod]
        public async Task AtualizarAnime_DeveRetornarBadRequestQuandoDadosForemInvalidos()
        {
            var animeId = 1;
            var anime = new Anime { Id = 1, Nome = null, Resumo = "Resumo do Anime1", Diretor = "Diretor1" }; // Anime com nome nulo
            _animeController.ModelState.AddModelError("Nome", "O campo Nome é obrigatório");
            var result = await _animeController.AtualizarAnime(animeId, anime);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }



        [TestMethod]
        public async Task ExcluirAnime_DeveRetornarNoContentQuandoAnimeExcluido()
        {
            _mockService.Setup(service => service.ExcluirAnime(1)).ReturnsAsync(true);
            var result = await _animeController.ExcluirAnime(1);
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
        }

        [TestMethod]
        public async Task ExcluirAnime_DeveRetornarNotFoundQuandoAnimeNaoExistir()
        {
            _mockService.Setup(service => service.ExcluirAnime(1)).ReturnsAsync(false);
            var result = await _animeController.ExcluirAnime(1);
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public async Task GetAnimes_DeveRetornarPaginacaoCorretaQuandoExistiremMaisAnimes()
        {
            var animes = new List<Anime>
    {
        new Anime { Id = 1, Nome = "Anime1", Resumo = "Resumo do Anime1", Diretor = "Diretor1" },
        new Anime { Id = 2, Nome = "Anime2", Resumo = "Resumo do Anime2", Diretor = "Diretor2" },
        new Anime { Id = 3, Nome = "Anime3", Resumo = "Resumo do Anime3", Diretor = "Diretor3" }
    };
            _mockService.Setup(service => service.GetAnimes(null, null, null, 2, 2)).ReturnsAsync(animes.Skip(2).Take(2)); // Página 2, 2 registros por página
            var result = await _animeController.GetAnimes(null, null, null, 2, 2);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultAnimes = okResult.Value as IEnumerable<Anime>;
            Assert.IsNotNull(resultAnimes);
            Assert.AreEqual(1, resultAnimes.Count()); 
            Assert.AreEqual(animes[2], resultAnimes.First()); 
        }

        [TestMethod]
        public async Task GetAnimes_DeveRetornarListaVaziaQuandoDiretorNaoExistir()
        {
            var diretorInexistente = "DiretorInexistente";
            _mockService.Setup(service => service.GetAnimes(diretorInexistente, null, null, 1, 10)).ReturnsAsync(new List<Anime>());
            var result = await _animeController.GetAnimes(diretorInexistente, null, null, 1, 10);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(0, (okResult.Value as List<Anime>).Count); 
        }

        [TestMethod]
        public async Task AtualizarAnime_DeveRetornarNotFoundQuandoAnimeNaoExistir()
        {
            var animeIdInexistente = 999; 
            var anime = new Anime { Id = animeIdInexistente, Nome = "AnimeInexistente", Resumo = "Resumo do AnimeInexistente", Diretor = "DiretorInexistente" };
            _mockService.Setup(service => service.AtualizarAnime(animeIdInexistente, anime)).ReturnsAsync((Anime)null);
            var result = await _animeController.AtualizarAnime(animeIdInexistente, anime);
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        private void ConfigureAuthentication()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlX3VzZXIiLCJqdGkiOiJjZGUzYTNlNy1hZWVmLTQxZTItYjJiMS1jOWQwZmE1NDg4ZDgiLCJpYXQiOjE3MTM0OTA3MDUsImV4cCI6MTcxMzQ5NDMwNSwiaXNzIjoibG9jYWxob3N0IiwiYXVkIjoibG9jYWxob3N0In0.gsaEOyu0WgKzNv4FUnKNiNg_0Edw7SKaMv4WA_4mr9s";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _animeController.ControllerContext = controllerContext;
        }

    }
}
