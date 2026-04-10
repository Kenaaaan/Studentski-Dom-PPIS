using StudentskiDom.Domain.Entities;

namespace StudentskiDom.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
