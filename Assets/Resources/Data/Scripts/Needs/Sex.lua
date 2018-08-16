function FindFulfillmentObject(entity)
	local spouse = entity:GetSpouse()
	
	--If we have a spouse, seek them out
	if spouse != nil then
		entity:Seek(spouse)
		return
	end
	
	local relationshipType = entity:GetCulture():RelationshipType()
	if relationshipType == "Polyamorous" or spouse == nil then			
		--Else, find someone nearby who you like the most
		local nearby = entity:SearchForMate()
		local highestLike = 0
		local chosenPartner = nil
		for key, target in ipairs(nearby) do
			local relationship = entity:HasRelationship(target:GUID())
			if relationship > highestLike then
				highestLike = relationship
				chosenPartner = target
			end
		end
		
		if chosenPartner != nil then
			entity:Seek(chosenPartner, "Sex")
		end
	end
	
	--If we can't find anyone, wander
	entity:Wander()
end