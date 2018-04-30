using System;

namespace CleanDds.Domain.Common
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
