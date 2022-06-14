---构造一个带继承功能的表
local class = function (super, className)
    local cls = {}
    if super then
        cls.super = super
        cls.className = className
        cls.__index = cls
        setmetatable(cls, {
            __index = super,
            __call = function (cls, ...)
                local instance = setmetatable({}, cls)
                local ctor = rawget(cls, "ctor")
                if ctor == nil then
                    return instance
                else
                    cls.ctor(instance, ...)
                    return instance
                end
            end
        })
    else
        cls.__index = function(t, key)
            local default = rawget(cls, key)
            t[key] = default
            return default
        end

        setmetatable(cls, {__call = function (cls, ...)
            local instance = { }
            setmetatable(instance, cls)
            local ctor = rawget(cls, "ctor")
            if ctor == nil then
                return instance
            else
                cls.ctor(instance, ...)
                return instance
            end
        end})
    end
    return cls
end

return class
