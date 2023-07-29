using Volo.Abp;

namespace LookOn.Exceptions;

public class LookOnBaseException : BusinessException
{
}

public class LookOnBizException : LookOnBaseException
{
}

public class LookOnSystemException : LookOnBaseException
{
}