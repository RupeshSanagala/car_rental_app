using Car_Rental_Backend_Application.Data.Entities;

public static class CarConverters
{
    public static Car CarRequestDtoToCar(CarRequestDto carDto)
    {
        return new Car
        {
            Brand = carDto.Brand,
            Model = carDto.Model,
            
            PricePerDay = (decimal)carDto.price,
            License_Plate = carDto.License_Plate,
            Avaliability_Status = carDto.Availability_Status
        };
    }

    public static CarResponseDto CarToCarResponseDto(Car car)
    {
        return new CarResponseDto
        {
            Car_ID = car.Car_ID,
            Brand = car.Brand,
            Model = car.Model,
          
            price = (double)car.PricePerDay,
            License_Plate = car.License_Plate,
            Availability_Status = car.Avaliability_Status
        };
    }
}