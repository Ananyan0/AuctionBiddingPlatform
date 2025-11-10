using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionBiddingPlatform.Core.Interfaces.IServices;

public interface IBidValidationService
{
    Task ValidateAsync(int itemId, decimal amount);
}