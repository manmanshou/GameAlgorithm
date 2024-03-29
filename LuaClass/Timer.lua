--[[
Copyright (c) 2010-2013 Matthias Richter
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
Except as contained in this notice, the name(s) of the above copyright holders
shall not be used in advertising or otherwise to promote the sale, use or
other dealings in this Software without prior written authorization.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
]]--
--[[

hump.timer
        ==========

::

Timer = require "hump.timer"

hump.timer offers a simple interface to schedule the execution of functions. It
is possible to run functions *after* and *for* some amount of time. For
example, a timer could be set to move critters every 5 seconds or to make the
player invincible for a short amount of time.

In addition to that, ``hump.timer`` offers various `tweening
<http://en.wikipedia.org/wiki/Inbetweening>`_ functions that make it
easier to produce `juicy games <http://www.youtube.com/watch?v=Fy0aCDmgnxg>`_.

**Example**::

function love.keypressed(key)
if key == ' ' then
Timer.after(1, function() print("Hello, world!") end)
end
end

function love.update(dt)
Timer.update(dt)
end

List of Functions
-----------------

* :func:`Timer.new() <Timer.new>`
* :func:`Timer.after(delay, func) <Timer.after>`
* :func:`Timer.script(func) <Timer.script>`
* :func:`Timer.every(delay, func, count) <Timer.every>`
* :func:`Timer.during(delay, func, after) <Timer.during>`
* :func:`Timer.cancel(handle) <Timer.cancel>`
* :func:`Timer.clear() <Timer.clear>`
* :func:`Timer.update(dt) <Timer.update>`
* :func:`Timer.tween(duration, subject, target, method, after, ...) <Timer.tween>`

Function Reference
------------------

.. function:: Timer.new()

:returns: A timer instance.


Creates a new timer instance that is independent of the global timer: It will
manage it's own list of scheduled functions and does not in any way affect the
the global timer. Likewise, the global timer does not affect timer instances.

.. note::
    If you don't need multiple independent schedulers, you can use the
global/default timer (see examples).

.. note::
Unlike the default timer, timer instances use the colon-syntax, i.e.,
you need to call ``instance:after(1, foo)`` instead of ``Timer.after(1,
foo)``.

**Example**::

menuTimer = Timer.new()


.. function:: Timer.after(delay, func)

:param number delay: Number of seconds the function will be delayed.
:param function func: The function to be delayed.
:returns: The timer handle. See also :func:`Timer.cancel`.


Schedule a function. The function will be executed after ``delay`` seconds have
elapsed, given that ``update(dt)`` is called every frame.

.. note::
There is no guarantee that the delay will not be exceeded, it is only
guaranteed that the function will *not* be executed *before* the delay has
passed.

``func`` will receive itself as only parameter. This is useful to implement
periodic behavior (see the example).

**Examples**::

-- grant the player 5 seconds of immortality
player.isInvincible = true
Timer.after(5, function() player.isInvincible = false end)

::

-- print "foo" every second. See also every()
Timer.after(1, function(func) print("foo") Timer.after(1, func) end)

::

--Using a timer instance:
menuTimer:after(1, finishAnimation)


.. function:: Timer.script(func)

:param function func: Script to execute.

Execute a function that can be paused without causing the rest of the program to
be suspended. ``func`` will receive a function - ``wait`` - to do interrupt the
script (but not the whole program) as only argument.  The function prototype of
wait is: ``wait(delay)``.

**Examples**::

Timer.script(function(wait)
print("Now")
wait(1)
print("After one second")
wait(1)
print("Bye!")
end)

::

-- useful for splash screens
Timer.script(function(wait)
Timer.tween(0.5, splash.pos, {x = 300}, 'in-out-quad')
wait(5) -- show the splash for 5 seconds
Timer.tween(0.5, slpash.pos, {x = 800}, 'in-out-quad')
end)

::

-- repeat something with a varying delay
Timer.script(function(wait)
while true do
spawn_ship()
wait(1 / (1-production_speed))
end
end)

::

-- jumping with timer.script
self.timers:script(function(wait)
local w = 1/12
self.jumping = true
Timer.tween(w*2, self, {z = -8}, "out-cubic", function()
Timer.tween(w*2, self, {z = 0},"in-cubic")
end)

self.quad = self.quads.jump[1]
wait(w)

self.quad = self.quads.jump[2]
wait(w)

self.quad = self.quads.jump[3]
wait(w)

self.quad = self.quads.jump[4]
wait(w)

self.jumping = false
self.z = 0
end)


.. function:: Timer.every(delay, func, count)

:param number delay: Number of seconds between two consecutive function calls.
:param function func: The function to be called periodically.
:param number count:  Number of times the function is to be called (optional).
:returns: The timer handle. See also :func:`Timer.cancel`.


Add a function that will be called ``count`` times every ``delay`` seconds.

If ``count`` is omitted, the function will be called until it returns ``false``
or :func:`Timer.cancel` or :func:`Timer.clear` is called on the timer instance.

**Example**::

-- toggle light on and off every second
Timer.every(1, function() lamp:toggleLight() end)

::

-- launch 5 fighters in quick succession (using a timer instance)
mothership_timer:every(0.3, function() self:launchFighter() end, 5)

::

-- flicker player's image as long as he is invincible
Timer.every(0.1, function()
player:flipImage()
return player.isInvincible
end)


.. function:: Timer.during(delay, func, after)

:param number delay: Number of seconds the func will be called.
:param function func: The function to be called on ``update(dt)``.
:param function after: A function to be called after delay seconds (optional).
:returns: The timer handle. See also :func:`Timer.cancel`.


Run ``func(dt)`` for the next ``delay`` seconds. The function is called every
time ``update(dt)`` is called. Optionally run ``after()`` once ``delay``
seconds have passed.

``after()`` will receive itself as only parameter.

.. note::
You should not add new timers in ``func(dt)``, as this can lead to random
crashes.

**Examples**::

-- play an animation for 5 seconds
Timer.during(5, function(dt) animation:update(dt) end)

::

-- shake the camera for one second
local orig_x, orig_y = camera:position()
Timer.during(1, function()
camera:lookAt(orig_x + math.random(-2,2), orig_y + math.random(-2,2))
end, function()
-- reset camera position
camera:lookAt(orig_x, orig_y)
end)

::

player.isInvincible = true
-- flash player for 3 seconds
local t = 0
player.timer:during(3, function(dt)
t = t + dt
player.visible = (t % .2) < .1
end, function()
-- make sure the player is visible after three seconds
player.visible = true
player.isInvincible = false
end)


.. function:: Timer.cancel(handle)

:param table handle:  The function to be canceled.

Prevent a timer from being executed in the future.

**Examples**::

function tick()
print('tick... tock...')
end
handle = Timer.every(1, tick)
-- later
Timer.cancel(handle) -- NOT: Timer.cancel(tick)

::

-- using a timer instance
function tick()
print('tick... tock...')
end
handle = menuTimer:every(1, tick)
-- later
menuTimer:cancel(handle)


.. function:: Timer.clear()

Remove all timed and periodic functions. Functions that have not yet been
executed will discarded.

**Examples**::

Timer.clear()

::

menuTimer:clear()


.. function:: Timer.update(dt)

:param number dt:  Time that has passed since the last ``update()``.

Update timers and execute functions if the deadline is reached. Call in
``love.update(dt)``.

**Examples**::

function love.update(dt)
do_stuff()
Timer.update(dt)
end

::

-- using hump.gamestate and a timer instance
function menuState:update(dt)
self.timers:update(dt)
end


.. function:: Timer.tween(duration, subject, target, method, after, ...)

:param number duration: Duration of the tween.
:param table subject: Object to be tweened.
:param table target: Target values.
:param string method: Tweening method, defaults to 'linear' (:ref:`see here
<tweening-methods>`, optional).
:param function after: Function to execute after the tween has finished
(optional).
:param mixed ...:  Additional arguments to the *tweening* function.
:returns: A timer handle.


`Tweening <http://en.wikipedia.org/wiki/Inbetweening>`_ (short for
in-betweening) is the process that happens between two defined states. For
example, a tween can be used to gradually fade out a graphic or move a text
message to the center of the screen. For more information why tweening should
be important to you, check out this great talk on `juicy games
<http://www.youtube.com/watch?v=Fy0aCDmgnxg>`_.

``hump.timer`` offers two interfaces for tweening: the low-level
:func:`Timer.during` and the higher level interface :func:`Timer.tween`.

To see which tweening methods hump offers, :ref:`see below <tweening-methods>`.

**Examples**::

function love.load()
color = {0, 0, 0}
Timer.tween(10, color, {255, 255, 255}, 'in-out-quad')
end

function love.update(dt)
Timer.update(dt)
end

function love.draw()
love.graphics.setBackgroundColor(color)
end

::

function love.load()
circle = {rad = 10, pos = {x = 400, y = 300}}
-- multiple tweens can work on the same subject
-- and nested values can be tweened, too
Timer.tween(5, circle, {rad = 50}, 'in-out-quad')
Timer.tween(2, circle, {pos = {y = 550}}, 'out-bounce')
end

function love.update(dt)
Timer.update(dt)
end

function love.draw()
love.graphics.circle('fill', circle.pos.x, circle.pos.y, circle.rad)
end

::

function love.load()
-- repeated tweening

circle = {rad = 10, x = 100, y = 100}
local grow, shrink, move_down, move_up
grow = function()
Timer.tween(1, circle, {rad = 50}, 'in-out-quad', shrink)
end
shrink = function()
Timer.tween(2, circle, {rad = 10}, 'in-out-quad', grow)
end

move_down = function()
Timer.tween(3, circle, {x = 700, y = 500}, 'bounce', move_up)
end
move_up = function()
Timer.tween(5, circle, {x = 200, y = 200}, 'out-elastic', move_down)
end

grow()
move_down()
end

function love.update(dt)
Timer.update(dt)
end

function love.draw()
love.graphics.circle('fill', circle.x, circle.y, circle.rad)
end



.. _tweening-methods:

Tweening methods
----------------

At the core of tweening lie interpolation methods. These methods define how the
output should look depending on how much time has passed. For example, consider
the following tween::

-- now: player.x = 0, player.y = 0
Timer.tween(2, player, {x = 2})
Timer.tween(4, player, {y = 8})

At the beginning of the tweens (no time passed), the interpolation method would
place the player at ``x = 0, y = 0``. After one second, the player should be at
``x = 1, y = 2``, and after two seconds the output is ``x = 2, y = 4``.

The actual duration of and time since starting the tween is not important, only
the fraction of the two. Similarly, the starting value and output are not
important to the interpolation method, since it can be calculated from the
start and end point. Thus an interpolation method can be fully characterized by
a function that takes a number between 0 and 1 and returns a number that
defines the output (usually also between 0 and 1). The interpolation function
must hold that the output is 0 for input 0 and 1 for input 1.

**hump** predefines several commonly used interpolation methods, which are
generalized versions of `Robert Penner's easing
functions <http://www.robertpenner.com/easing/>`_. Those are:

``'linear'``,
``'quad'``,
``'cubic'``,
``'quart'``,
``'quint'``,
``'sine'``,
``'expo'``,
``'circ'``,
``'back'``,
``'bounce'``, and
``'elastic'``.

It's hard to understand how these functions behave by staring at a graph, so
below are some animation examples. You can change the type of the tween by
changing the selections.

.. raw:: html

<div id="tween-graph"></div>
<script src="https://cdnjs.cloudflare.com/ajax/libs/d3/3.5.6/d3.min.js" charset="utf-8"></script>
<script src="_static/graph-tweens.js"></script>

Note that while the animations above show tweening of shapes, other attributes
(color, opacity, volume of a sound, ...) can be changed as well.


Custom interpolators
^^^^^^^^^^^^^^^^^^^^

.. warning:
This is a stub

You can add custom interpolation methods by adding them to the `tween` table::

Timer.tween.sqrt = function(t) return math.sqrt(t) end
-- or just Timer.tween.sqrt = math.sqrt

Access the your method like you would the predefined ones. You can even use the
modyfing prefixes::

Timer.tween(5, circle, {radius = 50}, 'in-out-sqrt')

You can also invert and chain functions::

outsqrt = Timer.tween.out(math.sqrt)
inoutsqrt = Timer.tween.chain(math.sqrt, outsqrt)
]]

---@class Timer
local Timer = {}

Timer.__index = Timer

local function _nothing_() end
local function ErrorWithStackTrace(msg)
    Error3(msg)
end

local function updateConditionHandle(handle, dt)
    -- handle: {
    --   condition = <function>:bool,
    --   onCompleted = <function>,
    --   limit = <number>,
    --   onLimit = <function>,
    -- }

    if handle.condition() then
        if handle.onCompleted ~= nil then
            local b, ret = xpcall(handle.onCompleted, ErrorWithStackTrace)
            --handle.onCompleted()
        end
        handle.count = 0
    else
        handle.limit = handle.limit - dt
        if handle.limit <= 0 then
            if handle.onLimit ~= nil then
                local b, ret = xpcall(handle.onLimit, ErrorWithStackTrace)
                --handle.onLimit()
            end
            handle.count = 0
        end
    end
end


local function updateTimerHandle(handle, dt)
    -- handle: {
    --   time = <number>,
    --   after = <function>,
    --   during = <function>,
    --   limit = <number>,
    --   count = <number>,
    -- }
    handle.time = handle.time + dt
    handle.during(dt, math.max(handle.limit - handle.time, 0))

    while handle.time >= handle.limit and handle.count > 0 do
        local b, res = xpcall(handle.after,ErrorWithStackTrace,handle.after)
        if b and res == false then
            handle.count = 0
            break
        end
        handle.time = handle.time - handle.limit
        handle.count = handle.count - 1
        if handle.limit == 0 then--0表示每帧是执行
            break
        end
    end
end

function Timer:update(dt)
    -- timers may create new timers, which leads to undefined behavior
    -- in pairs() - so we need to put them in a different table first
    local to_update = {}
    for handle in pairs(self.functions) do
        to_update[handle] = handle
    end

    for handle in pairs(to_update) do
        if self.functions[handle] then
            if handle.condition ~= nil then
                updateConditionHandle(handle, dt)
            else
                updateTimerHandle(handle, dt)
            end
            if handle.count == 0 then
                self.functions[handle] = nil
            end
        end
    end
end

function Timer:during(delay, during, after)
    local handle = { time = 0, during = during, after = after or _nothing_, limit = delay, count = 1 }
    self.functions[handle] = true
    return handle
end

function Timer:after(delay, func)
    return self:during(delay, _nothing_, func)
end

function Timer:every(delay, after, count, hasFirstTick)
    local count = count or math.huge -- exploit below: math.huge - 1 = math.huge
    local handle = { time = 0, during = _nothing_, after = after, limit = delay, count = count }
    if hasFirstTick and after ~= nil then
        after()
    end
    self.functions[handle] = true
    return handle
end

function Timer:frame(after, count)
    local count = count or math.huge -- exploit below: math.huge - 1 = math.huge
    local handle = { time = 0, during = _nothing_, after = after, limit = 0, count = count }
    self.functions[handle] = true
    return handle
end

function Timer:condition(checker, onCompleted, onLimit, limit)
    local handle = { condition = checker, onCompleted = onCompleted,  onLimit = onLimit or _nothing_, limit = limit or 15, count = 1}
    self.functions[handle] = true
    return handle
end

function Timer:cancel(handle)
    if(self.functions[handle]) then
        self.functions[handle] = nil
    end
end

function Timer:clear()
    self.functions = {}
end

function Timer:script(f)
    local co = coroutine.wrap(f)
    co(function(t)
        self:after(t, co)
        coroutine.yield()
    end)
end

Timer.tween = setmetatable({
    -- helper functions
    out = function(f) -- 'rotates' a function
        return function(s, ...) return 1 - f(1-s, ...) end
    end,
    chain = function(f1, f2) -- concatenates two functions
        return function(s, ...) return (s < .5 and f1(2*s, ...) or 1 + f2(2*s-1, ...)) * .5 end
    end,

    -- useful tweening functions
    linear = function(s) return s end,
    quad   = function(s) return s*s end,
    cubic  = function(s) return s*s*s end,
    quart  = function(s) return s*s*s*s end,
    quint  = function(s) return s*s*s*s*s end,
    sine   = function(s) return 1-math.cos(s*math.pi/2) end,
    expo   = function(s) return 2^(10*(s-1)) end,
    circ   = function(s) return 1 - math.sqrt(1-s*s) end,

    back = function(s,bounciness)
        bounciness = bounciness or 1.70158
        return s*s*((bounciness+1)*s - bounciness)
    end,

    bounce = function(s) -- magic numbers ahead
        local a,b = 7.5625, 1/2.75
        return math.min(a*s^2, a*(s-1.5*b)^2 + .75, a*(s-2.25*b)^2 + .9375, a*(s-2.625*b)^2 + .984375)
    end,

    elastic = function(s, amp, period)
        amp, period = amp and math.max(1, amp) or 1, period or .3
        return (-amp * math.sin(2*math.pi/period * (s-1) - math.asin(1/amp))) * 2^(10*(s-1))
    end,
}, {

    -- register new tween
    __call = function(tween, self, len, subject, target, method, after, ...)
        -- recursively collects fields that are defined in both subject and target into a flat list
        local function tween_collect_payload(subject, target, out)
            for k,v in pairs(target) do
                local ref = subject[k]
                assert(type(v) == type(ref), 'Type mismatch in field "'..k..'".')
                if type(v) == 'table' then
                    tween_collect_payload(ref, v, out)
                else
                    local ok, delta = pcall(function() return (v-ref)*1 end)
                    assert(ok, 'Field "'..k..'" does not support arithmetic operations')
                    out[#out+1] = {subject, k, delta}
                end
            end
            return out
        end

        method = tween[method or 'linear'] -- see __index
        local payload, t, args = tween_collect_payload(subject, target, {}), 0, {...}

        local last_s = 0
        return self:during(len, function(dt)
            t = t + dt
            local s = method(math.min(1, t/len), unpack(args))
            local ds = s - last_s
            last_s = s
            for _, info in ipairs(payload) do
                local ref, key, delta = unpack(info)
                ref[key] = ref[key] + delta * ds
            end
        end, after)
    end,

    -- fetches function and generated compositions for method `key`
    __index = function(tweens, key)
        if type(key) == 'function' then return key end

        assert(type(key) == 'string', 'Method must be function or string.')
        if rawget(tweens, key) then return rawget(tweens, key) end

        local function construct(pattern, f)
            local method = rawget(tweens, key:match(pattern))
            if method then return f(method) end
            return nil
        end

        local out, chain = rawget(tweens,'out'), rawget(tweens,'chain')
        return construct('^in%-([^-]+)$', function(...) return ... end)
                or construct('^out%-([^-]+)$', out)
                or construct('^in%-out%-([^-]+)$', function(f) return chain(f, out(f)) end)
                or construct('^out%-in%-([^-]+)$', function(f) return chain(out(f), f) end)
                or error('Unknown interpolation method: ' .. key)
    end})

-- Timer instancing
function Timer.new()
    return setmetatable({functions = {}, tween = Timer.tween}, Timer)
end

-- default instance
local default = Timer.new()

-- module forwards calls to default instance
local module = {}
for k in pairs(Timer) do
    if k ~= "__index" then
        module[k] = function(...) return default[k](default, ...) end
    end
end
module.tween = setmetatable({}, {
    __index = Timer.tween,
    __newindex = function(k,v) Timer.tween[k] = v end,
    __call = function(t, ...) return default:tween(...) end,
})

return setmetatable(module, {__call = Timer.new})