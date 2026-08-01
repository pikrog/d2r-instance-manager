using System;
using AvaloniaApplication1.Account;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region;

namespace AvaloniaApplication1.Instance;

public class InstanceValidation(AccountService accountService, RegionService regionService)
{
    public class GameInstanceValidationRules // todo: implement
    {
        
    }

    public class GameInstanceDraftValidator(AccountService accountService, RegionService regionService)
    {
        private readonly AccountService _accountService = accountService;
        private readonly RegionService _regionService = regionService;
        
        public void Validate(InstanceDraft draft)
        {
            if (draft.IsOnlineMode)
            {
                if (draft.AccountId is null)
                    throw new ArgumentException("Account id is required when using online mode");
            }
        }
    }
    
    private void Validate(InstanceDraft draft) // todo: move to validator
    {
        if (draft.IsOnlineMode)
        {
            if (draft.AccountId is null)
                throw new ArgumentException("Account id is required when using online mode");

            if (!accountService.Exists(draft.AccountId.Value))
                throw new ArgumentException(
                    $"Account with id {draft.AccountId} does not exist"); // todo: replace with a more specific exception

            if (draft.RegionId is null)
                throw new ArgumentException("Region id is required when using online mode");

            if (!regionService.Exists(draft.RegionId.Value))
                throw new ArgumentException($"Region with id {draft.RegionId} does not exist"); // todo: replace with a more specific exception
        }
        
        // todo: name uniqueness check -> in ConfigLoader
        if (string.IsNullOrWhiteSpace(draft.Name))
            throw new ArgumentException("Name is required");
    }
}