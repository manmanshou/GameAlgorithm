---触发绑定的表对应的回调函数
__sync = __sync or {}

return function()
    local sync = __sync
    for i, t in pairs(sync) do
        local __m_bind = t.__m_bind
        if(__m_bind ~= nil) then
            for _, func in pairs(t.__m_bind) do
                local _, err = pcall(func)
                if err then
                    Error3(err)
                end
            end
        end
        sync[i] = nil
    end
end