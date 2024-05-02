using Microsoft.AspNetCore.Mvc;
using Redis_DistributedCache.Model;
using Redis_DistributedCache.Service.Interface;

namespace Redis_DistributedCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Java", "C#", "C++", "SQL Server"
        ];

        private readonly ILogger<BlogController> _logger;
        private readonly IRedisCacheService _cacheService;

        public BlogController(ILogger<BlogController> logger,
            IRedisCacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet(Name = "GetBlog")]
        public IEnumerable<BlogModel> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<BlogModel>>("blog");
            if (cacheData != null)
            {
                return cacheData;
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            cacheData = Enumerable.Range(1, 5).Select(index => new BlogModel
            {
                Stock = Random.Shared.Next(-20, 55),
                BlogId = Random.Shared.Next(-20, 55),
                BlogDescription = Summaries[Random.Shared.Next(Summaries.Length)],
                BlogName = Summaries[Random.Shared.Next(Summaries.Length)],
                BlogSummary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToList();
            _cacheService.SetData<IEnumerable<BlogModel>>("blog", cacheData, expirationTime);
            return cacheData;
        }
    }
}
