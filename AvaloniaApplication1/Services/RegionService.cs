using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Services;

public class RegionService(ConfigService configService)
{
    private async Task AddAsync(RegionSnapshot snapshot) =>
        await configService.ChangeAsync(context => context.AddRegion(snapshot));

    private async Task UpdateAsync(RegionSnapshot snapshot) => 
        await configService.ChangeAsync(context => context.UpdateRegion(snapshot));

    public async Task SaveAsync(RegionDraft draft)
    {
        // Validate(draft);
        
        var id = draft.Id ?? Guid.NewGuid();
        var snapshot = new RegionSnapshot(id, draft.Name, draft.Address);
        
        if (draft.Id is null)
            await AddAsync(snapshot);
        else
            await UpdateAsync(snapshot);
    }

    public async Task RemoveAsync(Guid id) =>
        await configService.ChangeAsync(context => context.RemoveRegion(id));

    public RegionSnapshot GetSnapshot(Guid id) => configService.Config.GetRegion(id);
    
    public IReadOnlyList<RegionSnapshot> GetAllSnapshots() => configService.Config.GetAllRegions();
    
    public IReadOnlyList<RegionOption> GetOptions() => 
        configService.Config.GetAllRegions().Select(r => new RegionOption(r.Id, r.Name)).ToList();

    public bool Exists(Guid id) => configService.Config.RegionExists(id);
}