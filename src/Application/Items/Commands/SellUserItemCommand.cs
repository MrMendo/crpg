using Crpg.Application.Common;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Items.Commands;

public record SellUserItemCommand : IMediatorRequest
{
    public int UserItemId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<SellUserItemCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<SellUserItemCommand>();

        private readonly ICrpgDbContext _db;
        private readonly IItemModifierService _itemModifierService;
        private readonly Constants _constants;

        public Handler(ICrpgDbContext db, IItemModifierService itemModifierService, Constants constants)
        {
            _db = db;
            _itemModifierService = itemModifierService;
            _constants = constants;
        }

        public async Task<Result> Handle(SellUserItemCommand req, CancellationToken cancellationToken)
        {
            var userItem = await _db.UserItems
                .Include(ui => ui.User)
                .Include(ui => ui.BaseItem)
                .Include(ui => ui.EquippedItems)
                .FirstOrDefaultAsync(ui => ui.UserId == req.UserId && ui.Id == req.UserItemId, cancellationToken);

            if (userItem == null)
            {
                return new Result(CommonErrors.UserItemNotFound(req.UserItemId));
            }

            int price = _itemModifierService.ModifyItem(userItem.BaseItem!, userItem.Rank).Price;
            userItem.User!.Gold += (int)MathHelper.ApplyPolynomialFunction(price, _constants.ItemSellCostCoefs);
            _db.EquippedItems.RemoveRange(userItem.EquippedItems);
            _db.UserItems.Remove(userItem);

            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' sold item '{1}'", req.UserId, req.UserItemId);
            return new Result();
        }
    }
}