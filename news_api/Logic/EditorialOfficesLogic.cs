using news_api.Interfaces.Logic;
using news_api.Interfaces.Repositories;
using news_api.Models;

namespace news_api.Logic;

public class EditorialOfficesLogic : IEditorialOfficesLogic
{
    private readonly IEditorialOfficesRepository _editorialOffices;
    private readonly IApplicationUserLogic _applicationUserLogic;

    public EditorialOfficesLogic(IEditorialOfficesRepository editorialOfficesCollection,
        IApplicationUserLogic applicationUserLogic)
    {
        _editorialOffices = editorialOfficesCollection;
        _applicationUserLogic = applicationUserLogic;
    }

    public async Task<EditorialOffice?> GetByName(string editorialOfficeName)
    {
        var result = await _editorialOffices.GetByName(editorialOfficeName);
        if (result is null)
        {
            return null;
        }

        var owner = await _applicationUserLogic.GetManager().FindByIdAsync(result.OwnerId);
        result.OwnerInfo = $"{owner.Name} {owner.Surname}";

        return result;
    }
    
    public async Task<BaseResult> Create(EditorialOffice office)
    {
        try
        {
            if (await _applicationUserLogic.HasEditorialOffice(office.OwnerId ?? throw new InvalidOperationException()))
            {
                return new BaseResult() { Success = false, Message = "User already has an editorial office" };
            }

            _editorialOffices.Create(office);

            var result =
                await _applicationUserLogic.FindAndUpdate(office.OwnerId ?? throw new InvalidOperationException(),
                    new UserInfo { EditorialOfficeId = office.Id.ToString() });

            if (!result.Succeeded)
            {
                _editorialOffices.DeleteById(office.Id);
                return new BaseResult() { Success = false, Message = "Error while updating user info" };
            }

            return new BaseResult() { Success = true };
        }
        catch (Exception ex)
        {
            _editorialOffices.DeleteById(office.Id);
            return new BaseResult() { Success = false, Message = ex.Message };
        }
    }
    
    public async Task<bool> IsExists(string editorialOfficeName)
    {
        return await _editorialOffices.GetByName(editorialOfficeName) != null;
    }
}