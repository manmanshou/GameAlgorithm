
local __sync = __sync
local unbind = function (view, model)

    local __v_bind = rawget(view, "__v_bind")
    if __v_bind ~= nil then
        if model == nil then
            for _, v in pairs(__v_bind) do
                v()
            end
            for i, v in ipairs(__v_bind) do
                __v_bind[i] = nil
            end
        else
            if model.__m_bind ~= nil then
                --print("unbind 2 ---> [" .. tostring(model) .. "] to [" .. tostring(view) .. "]" .. debug.traceback());
                model.__m_bind[view] = nil
            end
        end
    end

end

return unbind

