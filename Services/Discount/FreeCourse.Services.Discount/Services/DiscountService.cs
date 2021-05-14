using Dapper;
using FreeCourse.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;

            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        }

        public async Task<Response<NoContent>> Delete(int id)
        {
            var deleteStatus = await _dbConnection.ExecuteAsync("DELETE FROM discount where id=@Id", new { Id=id});

            if (deleteStatus > 0)
                return Response<NoContent>.Success(204);

            return Response<NoContent>.Fail("Discount cannot be deleted", 500);
        }

        public async Task<Response<List<Models.Discount>>> GetAll()
        {
            var discounts = await _dbConnection.QueryAsync<Models.Discount>("Select * from discount");

            return Response<List<Models.Discount>>.Success(discounts.ToList(),200);
        }

        public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var discountsByCodeAndUserId =( await _dbConnection.QueryAsync<Models.Discount>("Select * from discount where code=@Code and userid=@UserId", new { Code = code, UserId = userId })).SingleOrDefault();

            if (discountsByCodeAndUserId == null)
                Response<Models.Discount>.Fail("Discount not found", 404);

            return Response<Models.Discount>.Success(discountsByCodeAndUserId, 200);
        }

        public async Task<Response<Models.Discount>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>("Select * from discount where Id=@Id",new {Id=id })).SingleOrDefault();

            if (discount==null)
                Response<Models.Discount>.Fail("Discount not found",404);

            return Response<Models.Discount>.Success(discount, 200);
        }

        public async Task<Response<NoContent>> Save(Models.Discount discount)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid,rate,code) VALUES(@UserId,@Rate,@Code)",discount);

            if (saveStatus > 0)
                return Response<NoContent>.Success(204);

            return Response<NoContent>.Fail("Discount cannot be saved", 500);

        }

        public async Task<Response<NoContent>> Update(Models.Discount discount)
        {
            var updateStatus = await _dbConnection.ExecuteAsync("UPDATE discount SET  userid=@UserId,rate=@Rate,code=@Code where id=@Id", new {Id=discount.Id,UserId=discount.UserId,Code=discount.Code,Rate=discount.Rate });

            if (updateStatus > 0)
                return Response<NoContent>.Success(204);

            return Response<NoContent>.Fail("Discount not found", 404);
        }
    }
}
