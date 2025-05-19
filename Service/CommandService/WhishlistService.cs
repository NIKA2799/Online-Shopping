using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;

namespace Service.CommandService
{
    public class WhishlistService : IWhishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WhishlistService(IUnitOfWork unitOfWork, IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public Wishlist GetWishlistByCustomerId(int cuctomerid)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return _unitOfWork.WishlistRepositorty.FindByCondition(c => c.CustomerId == cuctomerid).SingleOrDefault();
#pragma warning restore CS8603 // Possible null reference return.
        }
        public void AddWishlist(int customerId, WishlistItem item)
        {
            var whish = GetWishlistByCustomerId(customerId) ?? new Wishlist { CustomerId = customerId, Items = new List<WishlistItem>() };
            whish.Items.Add(item);
            _unitOfWork.WishlistRepositorty.Update(whish);
            _unitOfWork.SaveChanges();
        }
        public void RemoveFromWishlist(int customerId, int itemId)
        {
            var whishlist = GetWishlistByCustomerId(customerId);
            if (whishlist == null) return;
            var item = whishlist.Items.FirstOrDefault(w => w.Id == itemId);
            if (item != null)
            {
                whishlist.Items.Remove(item);
                _unitOfWork.WishlistRepositorty.Update(whishlist);
                _unitOfWork.SaveChanges();

            }
        }
        public void ClearWishlist(int customerId)
        {
            var whishlist = GetWishlistByCustomerId(customerId);
            if (whishlist != null)
            {
                whishlist.Items.Clear();
                _unitOfWork.WishlistRepositorty.Update(whishlist);
                _unitOfWork.SaveChanges();
            }
        }
        public WishlistModel get(int customerid)
        {
            var model = _unitOfWork.WishlistRepositorty.FindByCondition(w => w.CustomerId == customerid).SingleOrDefault();
            var whishlist = _mapper.Map<WishlistModel>(model);
            return whishlist;
        }
    }
}