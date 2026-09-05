using Grpc.Core;
using Google.Protobuf.WellKnownTypes;


namespace SensorGrpcServer.Services;

public class GreeterService(ILogger<GreeterService> logger, SensorStateService sensorStateService) : SensorService.SensorServiceBase
{   
   
    public override Task<SensorModificationUpdate> ModifySubscriptionToSensor(SensorModificationRequest request, ServerCallContext context)
    {   
        logger.LogInformation("The message is received from gear:{Gear}", request.Gear);
        var update =  new SensorModificationUpdate
        {
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
        };
        try
        {   
            if(request.Gear != sensorStateService.Gear)
            {   
                sensorStateService.Gear = request.Gear;
                sensorStateService.CurrentMessage = $"Gear is {request.Gear}";
                update.IsSuccess = true;
                update.UsingGear = request.Gear;
            }
            else
            {   
                sensorStateService.CurrentMessage = $"Gear is still the same {request.Gear}";
                update.IsSuccess = false;
                update.UsingGear = request.Gear;
            }
            
        }
        catch (Exception)
        {
            update.IsSuccess = false;
            update.UsingGear = request.Gear;
        }

        return Task.FromResult(update);
            
            
        
        
    }
    
    public override async Task SubscribeToSensor(SensorUpdateRequest request, IServerStreamWriter<SensorUpdate> responseStream, ServerCallContext context)
    {   
        
        logger.LogInformation("The message is received from {Name}", request);
        int counter = 0;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {   
                
                var update = new SensorUpdate
                {
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    Message = sensorStateService.CurrentMessage,
                    Speed = (double)(counter / 1.1),
                    IsActive = counter % 2 == 0,
                };

                if (counter % 3 == 0)
                {
                    update.Dir = new Direction { X = 1.7f, Y = 2.7f };    
                }
                
                
                
                
                await responseStream.WriteAsync(update);
                await Task.Delay(1000, context.CancellationToken); //1 sekunda, da je bolj berljivo za test
            }
            catch (OperationCanceledException)
            {
                
            }
            counter++;
        }
        

        
        
    }
}

/*

Task.FromResult(new SensorUpdate
        {
            Message = "Hello " + request
        });
*/