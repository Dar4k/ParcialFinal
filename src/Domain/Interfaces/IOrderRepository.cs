using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    void Save(Order order);
}