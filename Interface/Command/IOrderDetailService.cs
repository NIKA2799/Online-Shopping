using Interface.Model;

namespace Interface.Command
{
    public interface IOrderDetailService
    {
        int Insert(OrderDetailModel orderDetail);
        void Update(OrderDetailModel orderDetailModel, int id);
        public void Delete(int id);
        IEnumerable<OrderDetailModel> GetAll();
        OrderDetailModel GetById(int id);
    }
}
