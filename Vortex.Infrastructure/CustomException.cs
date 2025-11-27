namespace Vortex.Infrastructure.CustomException;

// 400 Bad Request
public class BadRequestException(string message) : Exception(message);

// 404 Not Found
public class NotFoundException(string message) : Exception(message);

// 401 Unauthorized
public class UnauthorizedException(string message) : Exception(message);

// 403 Forbidden
public class ForbiddenException(string message) : Exception(message);

// 409 Conflict (e.g., Duplicate email)
public class ConflictException(string message) : Exception(message);