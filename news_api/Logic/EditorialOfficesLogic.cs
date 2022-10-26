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

        result.OwnerInfo = await _applicationUserLogic.GetUserIdentity(result.OwnerId!);
        return result.ToBase();
    }

    public async Task<EditorialOffice?> GetById(string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return null;
        }

        var result = await _editorialOffices.GetById(guid);
        if (result is null)
        {
            return null;
        }

        result.OwnerInfo = await _applicationUserLogic.GetUserIdentity(result.OwnerId!);
        return result.ToBase();
    }

    public async Task<BaseResult> Create(EditorialOfficeDto officeDto)
    {
        try
        {
            if (await HasEditorialOffice(officeDto.OwnerId!))
            {
                return new BaseResult() { Success = false, Message = "User already has an editorial office" };
            }

            _editorialOffices.Create(officeDto);

            var result =
                await _applicationUserLogic.FindAndUpdate(officeDto.OwnerId ?? throw new InvalidOperationException(),
                    new UserInfo { EditorialOfficeId = officeDto.Id.ToString() });

            if (!result.Succeeded)
            {
                _editorialOffices.DeleteById(officeDto.Id);
                return new BaseResult() { Success = false, Message = "Error while updating user info" };
            }

            return new BaseResult() { Success = true };
        }
        catch (Exception ex)
        {
            _editorialOffices.DeleteById(officeDto.Id);
            return new BaseResult() { Success = false, Message = ex.Message };
        }
    }

    public async Task<BaseResult> Delete(string userId, string officeId)
    {
        var userUpdated = await _applicationUserLogic.DeleteEditorialOffice(userId);

        if (!userUpdated.Succeeded)
        {
            return new BaseResult() { Success = false, Message = "Error while updating user info" };
        }

        if (!Guid.TryParse(officeId, out var guid))
        {
            return new BaseResult() { Success = false, Message = "Invalid id" };
        }

        try
        {
            _editorialOffices.DeleteById(guid);
        }
        catch (Exception)
        {
            return new BaseResult() { Success = false, Message = "Something went wrong" };
        }

        return new BaseResult() { Success = true };
    }

    public async Task<bool> IsExists(string editorialOfficeName) =>
        await _editorialOffices.GetByName(editorialOfficeName) != null;

    public async Task<bool> HasEditorialOffice(string userId) =>
        await _applicationUserLogic.HasEditorialOffice(userId);
}