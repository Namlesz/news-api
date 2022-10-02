using news_api.Models;
using news_api.Repositories;

namespace news_api.Logic;

public class EditorialOfficesLogic
{
    private readonly EditorialOfficesRepository _editorialOffices;
    private readonly ApplicationUserLogic _applicationUserLogic;

    public EditorialOfficesLogic(EditorialOfficesRepository editorialOfficesCollection,
        ApplicationUserLogic applicationUserLogic)
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
}