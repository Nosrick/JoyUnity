function FindFulfillmentObject(entity)
	local targets = entity:SearchForObject("Furniture")
	local bestBed = 0
	local chosenBed = nil
	
	for key, target in ipairs(targets) do
		if target.Value > bestBed then
			bestBed = target.Value
			chosenBed = target
		end
	end
	
	if chosenBed != nil then
		entity:Seek(chosenBed, "Sleep")
		return
	end
	
	entity:FulfillNeed("Sleep", 16, 420)
end