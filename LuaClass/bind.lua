local bindImpl = function(view, model, func)
    local __v_bind = rawget(view, "__v_bind")
    if __v_bind == nil then
        __v_bind = {}
        rawset(view, "__v_bind", __v_bind)
    end

    local __m_bind = rawget(model, "__m_bind")
    if (__m_bind == nil) then
        __m_bind = { }
        rawset(model, "__m_bind", __m_bind)
    end

    --print("rebind ---> [" .. tostring(model) .. "] to [" .. tostring(view) .. "]" .. debug.traceback());
    __m_bind[view] = func

    --print("bind -----> [" .. tostring(model) .. "] to [" .. tostring(view) .. "]" .. debug.traceback());
    table.insert(__v_bind, function()

        --print("unbind ---> [" .. tostring(model) .. "] to [" .. tostring(view) .. "]" .. debug.traceback());
        __m_bind[view] = nil
    end)

    func(model)

end

---bind 接口提供了让view关联多个model表，后续当model数据变化时，sync调用对应的回调函数并传入model作为参数
---最后一个参数是回调函数，view和回调函数之间的多个表是model
local bind = function(view, model, ...)

    local arg = { ... }
    local func = arg[#arg]

    bindImpl(view, model, func)
    for i = 1, #arg - 1 do
        bindImpl(view, arg[i], func)
    end

end

return bind

